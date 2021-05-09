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

        public bool AddRoom(AddRoomViewModel model)
        {
            var newRoom = new Room
            {
                RoomId = new Guid(),
                FloorNumber = model.Floor,
                RoomName = model.RoomName,
                Triggers = new List<Trigger>()
            };
            _context.Add<Room>(newRoom);
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

        public void ChangeLightState(Light light, Light.LightState lightState, int brightness, double energyUsage)
        {
            var lightToUpdate = _context.Lights.Where(l => l.LightId == light.LightId).FirstOrDefault();
            lightToUpdate.CurrentState = lightState;
            lightToUpdate.CurrentBrightness = brightness;
            var localTime = DateTime.Now;
            var newLightEvent = new LightEvent
            {
                LightEventId = new Guid(),
                Lightid = lightToUpdate,
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
            _context.Remove<Light>(lightToDelete);
            return _context.SaveChanges() > 0;

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

        public IEnumerable<User> GetAllUser()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }

        public Light GetLightById(Guid lightId)
        {
            return _context.Lights.Where(l => l.LightId == lightId).FirstOrDefault();
        }

        public Light GetLightFromMqttMessage(string roomName, int floorNumber, int controllerPin, string controllerName)
        {
            var room = _context.Rooms.Where(r => r.RoomName == roomName && r.FloorNumber == floorNumber).FirstOrDefault();
            var trigger = _context.Triggers.FromSqlRaw($"Select * FROM \"Triggers\" WHERE \"RoomId\" = '{room.RoomId}'").FirstOrDefault();
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
