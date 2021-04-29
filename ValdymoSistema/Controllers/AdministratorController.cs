using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValdymoSistema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Models;

namespace ValdymoSistema.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private readonly IDatabaseController _database;
        private readonly MqttClient _mqttClient;

        public AdministratorController(IDatabaseController database, MqttClient mqttClient)
        {
            _database = database;
            _mqttClient = mqttClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddRoom()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRoom(AddRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_database.AddRoom(model))
                {
                    ViewBag.Message = "Patalpa pridėta sėkmingai";
                    return View();
                }
            }
            return View();
        }

        public IActionResult AddTrigger()
        {
            var rooms = _database.GetAllRooms();
            var model = new AddTriggerViewModel { Rooms = rooms.ToList() };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddTrigger(AddTriggerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_database.AddTrigger(model))
                {
                    ViewBag.Message = "Jungiklis pridėtas sėkmingai";
                    var rooms = _database.GetAllRooms();
                    var newModel = new AddTriggerViewModel { Rooms = rooms.ToList() };
                    return View(newModel);
                }
            }
            return View();
        }

        public IActionResult AddLight()
        {
            var rooms = _database.GetAllRooms();
            var model = new AddLightViewModel { Rooms = rooms.ToList() };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddLight(AddLightViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_database.AddLight(model))
                {
                    ViewBag.Message = "Šviestuvas pridėtas sėkmingai";
                    var rooms = _database.GetAllRooms();
                    var newModel = new AddLightViewModel { Rooms = rooms.ToList() };
                    return View(newModel);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UnblockLight([FromForm]Guid lightId, string triggerName, string roomName, int floorNumber)
        {
            var mqttTopic = $"{floorNumber}/{roomName}/{triggerName}";
            var light = _database.GetLightById(lightId);
            var lightPin = light.ControllerPin;
            var mqttMessage = $"Unblock;{lightPin}";
            _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            mqttMessage = $"Off;{lightPin}";
            _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            TempData["Message"] = "Šviesa atblokuota sėkmingai";
            return RedirectToAction("Index", "Worker");
        }

        [HttpPost]
        public IActionResult DeleteLight([FromForm]Guid lightId)
        {
            var viewBagMessage = "";
            if (_database.DeleteLight(lightId))
            {
                
                viewBagMessage = "Šviesa ištrinta sėkmingai";
            }
            else
            {
                viewBagMessage = "Įvyko klaida trinant šviestuvą";
            }
            TempData["Message"] = viewBagMessage;
            return RedirectToAction("Index", "Worker");
        }
    }
}
