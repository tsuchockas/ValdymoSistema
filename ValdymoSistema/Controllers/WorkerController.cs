using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
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
        public async Task<IActionResult> Index()
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

            var lightsModel = new LightsViewModel { 
                Lights = lights.ToList(),
                Triggers = triggers.Distinct().ToList(),
                Rooms = rooms.Distinct().OrderBy(r => r.FloorNumber).ThenBy(r => r.RoomName).ToList()
            };
            return View(lightsModel);
        }

        [HttpPost]
        public async Task<IActionResult> TurnOnLight([FromForm]Guid lightId, int brightness)
        {
            await TurnOnLightAsync(lightId, brightness);
            Thread.Sleep(500);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TurnOffLight([FromForm]Guid lightId)
        {
            await TurnOffLightAsync(lightId);
            Thread.Sleep(500);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BlockLight([FromForm]Guid lightId)
        {
            await BlockLightAsync(lightId);
            Thread.Sleep(500);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOnRoom([FromForm] Guid roomId)
        {
            var lightsToTurnOn = _database.GetAllLightInRoom(roomId);
            foreach (var light in lightsToTurnOn)
            {
                await TurnOnLightAsync(light.LightId, 100);
            }
            Thread.Sleep(500);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TurnOffRoom([FromForm] Guid roomId)
        {
            var lightsToTurnOn = _database.GetAllLightInRoom(roomId);
            foreach (var light in lightsToTurnOn)
            {
                await TurnOffLightAsync(light.LightId);
            }
            Thread.Sleep(500);
            return RedirectToAction("Index");
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
