using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Data.Entities;
using ValdymoSistema.Models;

namespace ValdymoSistema.Controllers
{
    public class DatabaseController : IDatabaseController
    {
        private readonly ApplicationDbContext _context;

        public DatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddLight(AddLightViewModel model)
        {
            var newLight = new Light
            {
                LightId = new Guid(),
                ControllerPin = model.ControllerPin,
                CurrentState = Light.LightState.Off
            };
            var trigger = _context.Triggers.Where(t => t.TriggerId.ToString().Equals(model.TriggerId)).FirstOrDefault();
            trigger.Lights = _context.Lights.FromSqlRaw($"Select * From \"Lights\" Where \"TriggerId\" = '{model.TriggerId}'").ToList();
            trigger.Lights.Add(newLight);
            _context.Update<Trigger>(trigger);
            return _context.SaveChanges() > 0;
        }

        public bool AddLightFromMqttMessage(AddLightFromMqttMessage model)
        {
            var room = _context.Rooms.Where(r => r.FloorNumber == model.FloorNumber && r.RoomName.Equals(model.RoomName)).FirstOrDefault();
            var newRoomIsAdded = false;
            var newLight = new Light
            {
                LightId = new Guid(),
                ControllerPin = model.LightPin,
                CurrentBrightness = 0,
                CurrentState = Light.LightState.Off,
                Users = new List<User>()
            };
            if (room == null)
            {
                var lights = new List<Light>();
                lights.Add(newLight);
                var triggers = new List<Trigger>();
                triggers.Add(new Trigger
                {
                    TriggerId = new Guid(),
                    TriggerName = model.TriggerName,
                    Lights = lights
                });
                room = new Room
                {
                    RoomId = new Guid(),
                    RoomName = model.RoomName,
                    FloorNumber = model.FloorNumber,
                    Triggers = triggers
                };
                _context.Add(room);
                newRoomIsAdded = true;
            }
            if (!newRoomIsAdded)
            {
                room.Triggers = _context.Triggers
                .FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").ToList();
                var triggerInRoom = _context.Triggers
                .FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}' AND \"TriggerName\" = '{model.TriggerName}'")
                .FirstOrDefault();
                if (triggerInRoom == null)
                {
                    var lights = new List<Light>();
                    lights.Add(newLight);
                    triggerInRoom = new Trigger
                    {
                        Lights = lights,
                        TriggerName = model.TriggerName,
                        TriggerId = new Guid()
                    };
                    room.Triggers.Add(triggerInRoom);
                    _context.Update(room);
                }
                triggerInRoom.Lights = _context.Lights.FromSqlRaw($"Select * From \"Lights\" Where \"TriggerId\" = '{triggerInRoom.TriggerId}'").ToList();
                if (triggerInRoom.Lights.Where(l => l.ControllerPin == model.LightPin).FirstOrDefault() == null)
                {
                    triggerInRoom.Lights.Add(newLight);
                    _context.Update(triggerInRoom);
                }
            }
            return _context.SaveChanges() > 0;
        }

        public bool AddRoom(AddRoomViewModel model)
        {
            var newRoom = new Room
            {
                RoomId = new Guid(),
                FloorNumber = model.Floor,
                RoomName = model.RoomName,
                Triggers = new List<Trigger>()
            };
            var room = _context.Rooms.Where(r => r.FloorNumber == model.Floor && r.RoomName.Equals(model.RoomName)).FirstOrDefault();
            if (room == null)
            {
                _context.Add<Room>(newRoom);
            }
            return _context.SaveChanges() > 0;
        }

        public bool AddTrigger(AddTriggerViewModel model)
        {
            var room = _context.Rooms.Where(r => r.RoomId.ToString().Equals(model.RoomId)).FirstOrDefault();
            var newTrigger = new Trigger
            {
                TriggerId = new Guid(),
                TriggerName = model.ControllerName,
                Lights = new List<Light>()
            };
            room.Triggers = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").ToList();
            room.Triggers.Add(newTrigger);
            _context.Update<Room>(room);
            return _context.SaveChanges() > 0;
        }

        public bool AssignLightsToUser(List<Light> lights, string username)
        {
            var user = _context.Users.Where(u => u.UserName.Equals(username)).FirstOrDefault();
            if (user.Lights == null)
            {
                user.Lights = new List<Light>();
            }
            foreach (var light in lights)
            {
                if (!user.Lights.Contains(light))
                {
                    user.Lights.Add(light);
                }
            }
            _context.Update(user);
            return _context.SaveChanges() > 0;
        }

        public void ChangeLightState(Light light, Light.LightState lightState, int brightness, double energyUsage)
        {
            var lightToUpdate = _context.Lights.Where(l => l.LightId == light.LightId).FirstOrDefault();
            lightToUpdate.CurrentState = lightState;
            lightToUpdate.CurrentBrightness = brightness;
            var localTime = DateTime.Now;
            var newLightEvent = new LightEvent
            {
                LightEventId = new Guid(),
                Light = lightToUpdate,
                Brightness = brightness,
                CurrentLightState = lightState,
                DateTime = DateTime.Now,
                EnergyUsage = energyUsage
            };
            _context.Add(newLightEvent);
            _context.Update(lightToUpdate);
            _context.SaveChanges();
        }

        public bool DeleteLight(Guid lightId)
        {
            var lightToDelete = _context.Lights.Where(l => l.LightId == lightId).FirstOrDefault();
            var lightEvents = _context.LightEvents.Where(evt => evt.Light == lightToDelete).ToList();
            _context.RemoveRange(lightEvents);
            _context.Remove(lightToDelete);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<Light> GetAllLightInRoom(Guid roomId)
        {
            var lightsToReturn = new List<Light>();
            var room = _context.Rooms.Where(r => r.RoomId == roomId).FirstOrDefault();
            room.Triggers = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").ToList();
            foreach (var trigger in room.Triggers)
            {
                lightsToReturn.AddRange(_context.Lights.FromSqlRaw($"Select * From \"Lights\" Where \"TriggerId\" = '{trigger.TriggerId}'").ToList());
            }
            return lightsToReturn;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            //var roomsToReturn = new List<Room>();
            var rooms = _context.Rooms.OrderBy(r => r.RoomId).ToList();
            foreach (var room in rooms)
            {
                room.Triggers = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").ToList();
            }
            return rooms;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }

        public Dictionary<Light, List<LightEvent>> GetBurntEvents()
        {
            var dictToReturn = new Dictionary<Light, List<LightEvent>>();
            var listForOneLight = new List<LightEvent>();
            var allLights = _context.Lights.OrderBy(l => l.LightId).ToList();
            foreach (var light in allLights)
            {
                listForOneLight.AddRange(_context.LightEvents.Where(l => l.Light == light && l.CurrentLightState == Light.LightState.Burnt)
                    .OrderBy(l => l.DateTime).ToList());
                dictToReturn.Add(light, listForOneLight);
                listForOneLight = new List<LightEvent>();
            }
            return dictToReturn;
        }

        public Dictionary<Light, List<LightEvent>> GetEnergyUsage(GetEnergyUsageViewModel model)
        {
            var room = _context.Rooms.Where(r => r.RoomId == model.RoomId).FirstOrDefault();
            room.Triggers = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").ToList();
            var listToReturn = new Dictionary<Light, List<LightEvent>>();
            var lightEventList = new List<LightEvent>();
            foreach (var trigger in room.Triggers)
            {
                trigger.Lights = _context.Lights.FromSqlRaw($"Select * From \"Lights\" Where \"TriggerId\" = '{trigger.TriggerId}'").ToList();
                foreach (var light in trigger.Lights)
                {
                    if (lightEventList.Count > 0)
                    {
                        lightEventList = new List<LightEvent>();
                    }
                    var lightOnEvents = _context.LightEvents.Where(l => l.Light.Equals(light)
                    && l.CurrentLightState == Light.LightState.On
                    && l.DateTime >= model.DateFrom
                    && l.DateTime <= model.DateTo).OrderBy(l => l.DateTime).ToList();
                    var lightOffEvents = new List<LightEvent>();
                    lightOffEvents = _context.LightEvents.Where(l => l.Light.Equals(light)
                    && l.CurrentLightState == Light.LightState.Off
                    && l.DateTime >= model.DateFrom
                    && l.DateTime <= model.DateTo).OrderBy(l => l.DateTime).ToList();
                    if (lightOffEvents.Count == 0 || (lightOnEvents.ElementAt(lightOnEvents.Count - 1).DateTime > lightOffEvents.ElementAt(lightOffEvents.Count - 1).DateTime))
                    {
                        lightOffEvents.Add(new LightEvent
                        {
                            LightEventId = new Guid(),
                            CurrentLightState = Light.LightState.Off,
                            DateTime = model.DateTo,
                            Brightness = 0,
                            EnergyUsage = 0.0,
                            Light = light
                        });
                    }
                    lightOnEvents.AddRange(lightOffEvents);
                    var allLightEvents = lightOnEvents.OrderBy(evt => evt.DateTime).ToList();
                    if (allLightEvents.Count > 1)
                    {
                        listToReturn.Add(light, allLightEvents);
                    }
                }
            }
            return listToReturn;
        }

        public Light GetLightById(Guid lightId)
        {
            return _context.Lights.Where(l => l.LightId == lightId).FirstOrDefault();
        }

        public Light GetLightFromMqttMessage(string roomName, int floorNumber, int controllerPin, string controllerName)
        {
            var room = _context.Rooms.Where(r => r.RoomName == roomName && r.FloorNumber == floorNumber).FirstOrDefault();
            var trigger = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}' AND \"TriggerName\" = '{controllerName}'").FirstOrDefault();
            var lightToReturn = _context.Lights.FromSqlRaw($"Select * From \"Lights\" Where \"TriggerId\" = '{trigger.TriggerId}' AND \"ControllerPin\" = {controllerPin}").FirstOrDefault();
            return lightToReturn;
        }

        public IEnumerable<Light> GetLightsForUser(string UserName)
        {
            if (UserIsAdmin(UserName))
            {
                return _context.Lights.OrderBy(l => l.LightId).ToList();
            }
            var user = _context.Users.Where(u => u.UserName.Equals(UserName)).FirstOrDefault();
            return _context.Lights.Where(l => l.Users.Contains(user)).ToList();
        }

        public IEnumerable<string> GetOperatorEmails()
        {
            var operatorRoleId = _context.Roles.Where(r => r.Name.Equals("Operator")).FirstOrDefault().Id;
            var operatorIds = _context.UserRoles.Where(u => u.RoleId.Equals(operatorRoleId)).ToList();
            var operatorList = new List<User>();
            foreach (var opId in operatorIds)
            {
                operatorList.Add(_context.Users.Where(op => op.Id.Equals(opId.UserId)).FirstOrDefault());
            }

            return operatorList.Select(op => op.Email)?.ToList();
        }

        public Room GetRoom(Guid roomId)
        {
            return _context.Rooms.Where(r => r.RoomId.Equals(roomId)).FirstOrDefault();
        }

        public Room GetRoomForTrigger(Trigger trigger)
        {
            return _context.Rooms.Where(r => r.Triggers.Contains(trigger)).FirstOrDefault();
        }

        public Trigger GetTriggerForLight(Light light)
        {
            return _context.Triggers.Where(t => t.Lights.Contains(light)).FirstOrDefault();
        }

        private bool UserIsAdmin(string UserName)
        {
            var adminRoleId = _context.Roles.Where(r => r.Name.Equals("Administrator")).FirstOrDefault().Id;
            var user = _context.Users.Where(u => u.UserName.Equals(UserName)).FirstOrDefault();
            var admin = _context.UserRoles.Where(u => u.UserId.Equals(user.Id) && u.RoleId.Equals(adminRoleId)).FirstOrDefault();
            return admin != null;
        }
    }
}
