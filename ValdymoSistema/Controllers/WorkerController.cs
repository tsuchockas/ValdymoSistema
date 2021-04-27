using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Rooms = rooms.Distinct().ToList()
            };
            return View(lightsModel);
        }

        [HttpPost]
        public async Task<IActionResult> TurnOnLight([FromForm]Guid lightId, string triggerName, string roomName, int floorNumber)
        {
            var mqttTopic = $"{floorNumber}/{roomName}/{triggerName}";
            var light = _database.GetLightById(lightId);
            var lightPin = light.ControllerPin;
            var mqttMessage = $"On;{lightPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TurnOffLight([FromForm]Guid lightId, string triggerName, string roomName, int floorNumber)
        {
            var mqttTopic = $"{floorNumber}/{roomName}/{triggerName}";
            var light = _database.GetLightById(lightId);
            var lightPin = light.ControllerPin;
            var mqttMessage = $"Off;{lightPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            return RedirectToAction("Index");
        }
    }
}
