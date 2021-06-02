using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Data.Entities;
using ValdymoSistema.Models;
using ValdymoSistema.Services;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Controllers
{
    [Authorize]
    public class WorkerController : Controller
    {
        private MqttClient _mqttClient;
        private readonly IDatabaseController _database;
        private readonly UserManager<User> _userManager;

        public WorkerController(MqttClient mqttClient, IDatabaseController database, UserManager<User> userManager)
        {
            _mqttClient = mqttClient;
            _database = database;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int floorNumber, string roomName, string roomType)
        {
            ViewBag.Message = TempData["Message"];
            var currentUser = User.Identity.Name;
            var lights = _database.GetLightsForUser(currentUser);
            var triggers = new List<Trigger>();
            var rooms = new List<Room>();
            foreach (var light in lights)
            {
                var triggerToAdd = _database.GetTriggerForLight(light);
                triggers.Add(triggerToAdd);
                rooms.Add(_database.GetRoomForTrigger(triggerToAdd));
            }
            rooms = rooms.Distinct().OrderBy(r => r.FloorNumber).ThenBy(r => r.RoomName).ToList();
            var roomAndState = new Dictionary<Guid, string>();
            foreach (var room in rooms)
            {
                roomAndState.Add(room.RoomId, GetRoomState(room));
            }
            switch (roomType)
            {
                case "On":
                    var roomsWithOnLights = roomAndState.Where(r => r.Value.Equals("On")).ToList();
                    rooms.Clear();
                    foreach (var room in roomsWithOnLights)
                    {
                        rooms.Add(_database.GetRoom(room.Key));
                    }
                    break;
                case "Off":
                    var roomsWithOffLights = roomAndState.Where(r => r.Value.Equals("Off")).ToList();
                    rooms.Clear();
                    foreach (var room in roomsWithOffLights)
                    {
                        rooms.Add(_database.GetRoom(room.Key));
                    }
                    break;
                case "Burnt":
                    var roomsWithBurntLights = roomAndState.Where(r => r.Value.Equals("Burnt")).ToList();
                    rooms.Clear();
                    foreach (var room in roomsWithBurntLights)
                    {
                        rooms.Add(_database.GetRoom(room.Key));
                    }
                    break;
                case "Default":
                    break;
                default:
                    break;
            }
            if (floorNumber > 0)
            {
                var roomsWithFloorNumber = rooms.Where(r => r.FloorNumber != floorNumber).ToList();
                foreach (var room in roomsWithFloorNumber)
                {
                    rooms.Remove(room);
                }
            }
            if (!String.IsNullOrEmpty(roomName))
            {
                var roomsWithoutName = rooms.Where(r => r.RoomName.Contains(roomName, StringComparison.InvariantCultureIgnoreCase) == false).ToList();
                foreach (var room in roomsWithoutName)
                {
                    rooms.Remove(room);
                }
            }
            var lightsModel = new LightsViewModel { 
                Lights = lights.ToList(),
                Triggers = triggers.Distinct().ToList(),
                Rooms = rooms.Distinct().OrderBy(r => r.FloorNumber).ThenBy(r => r.RoomName).ToList(),
                RoomAndState = roomAndState
            };
            return View(lightsModel);
        }

        private string GetRoomState(Room room)
        {
            var roomState = "Off";
            var lightsLit = 0;
            var lightsBurnt = 0;
            foreach (var trigger in room.Triggers)
            {
                foreach (var light in trigger.Lights)
                {
                    switch (light.CurrentState)
                    {
                        case LightState.On:
                            lightsLit++;
                            break;
                        case LightState.Burnt:
                            lightsBurnt++;
                            break;
                        default:
                            break;
                    }
                }
            }
            if(lightsLit > 0)
            {
                roomState = "On";
            }
            if (lightsBurnt > 0)
            {
                roomState = "Burnt";
            }
            return roomState;
        }

        [HttpPost]
        public async Task<IActionResult> TurnOnLight([FromForm]Guid lightId, int brightness)
        {
            await TurnOnLightAsync(lightId, brightness);
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TurnOffLight([FromForm]Guid lightId)
        {
            await TurnOffLightAsync(lightId);
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOffRooms([FromForm] List<Guid> RoomIds)
        {
            foreach (var roomId in RoomIds)
            {
                await TurnOffRooms(roomId);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TurnOnRooms([FromForm] List<Guid> RoomIds)
        {
            foreach (var roomId in RoomIds)
            {
                await TurnOnRooms(roomId);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BlockLight([FromForm]Guid lightId)
        {
            await BlockLightAsync(lightId);
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOnRoom([FromForm] Guid roomId)
        {
            await TurnOnRooms(roomId);
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOffRoom([FromForm] Guid roomId)
        {
            await TurnOffRooms(roomId);
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOnMotionSensor(string roomName, int floorNumber, string triggerName)
        {
            var mqttTopic = $"{floorNumber}/{roomName}/{triggerName}";
            var mqttMessage = $"Motion";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            return RedirectToAction("Index");
        }

        private async Task TurnOffRooms(Guid RoomId)
        {
            var lightsToTurnOff = _database.GetAllLightInRoom(RoomId);
            foreach (var light in lightsToTurnOff)
            {
                await TurnOffLightAsync(light.LightId);
            }
        }

        private async Task TurnOnRooms(Guid RoomId)
        {
            var lightsToTurnOn = _database.GetAllLightInRoom(RoomId);
            foreach (var light in lightsToTurnOn)
            {
                await TurnOnLightAsync(light.LightId, 100);
            }
        }

        private async Task TurnOnLightAsync(Guid lightId, int brightness)
        {
            var light = _database.GetLightById(lightId);
            var trigger = _database.GetTriggerForLight(light);
            var room = _database.GetRoomForTrigger(trigger);
            var mqttTopic = $"{room.FloorNumber}/{room.RoomName}/{trigger.TriggerName}";
            var mqttMessage = $"On;{light.ControllerPin};{brightness}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
        }

        private async Task TurnOffLightAsync(Guid lightId)
        {
            var light = _database.GetLightById(lightId);
            var trigger = _database.GetTriggerForLight(light);
            var room = _database.GetRoomForTrigger(trigger);
            var mqttTopic = $"{room.FloorNumber}/{room.RoomName}/{trigger.TriggerName}";
            var mqttMessage = $"Off;{light.ControllerPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
        }

        private async Task BlockLightAsync(Guid lightId)
        {
            var light = _database.GetLightById(lightId);
            var trigger = _database.GetTriggerForLight(light);
            var room = _database.GetRoomForTrigger(trigger);
            var mqttTopic = $"{room.FloorNumber}/{room.RoomName}/{trigger.TriggerName}";
            var mqttMessage = $"Block;{light.ControllerPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
        }
    }
}
