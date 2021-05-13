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
using Microsoft.AspNetCore.Mvc.RazorPages;
using ValdymoSistema.Data.Entities;

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
            ViewBag.Message = TempData["Message"];
            var currentUser = User.Identity.Name;
            var rooms = _database.GetAllRooms().ToList();
            var users = _database.GetAllUsers().ToList();
            users.RemoveAll(u => u.UserName.Equals(currentUser));
            var model = new AssignLightsToUserViewModel
            {
                RegisteredRooms = rooms,
                RegisteredUsers = users
            };
            return View(model);
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
                    return View(new AddRoomViewModel());
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
            var rooms = _database.GetAllRooms();
            var newModel = new AddTriggerViewModel { Rooms = rooms.ToList() };
            if (ModelState.IsValid)
            {
                if (_database.AddTrigger(model))
                {
                    ViewBag.Message = "Jungiklis pridėtas sėkmingai";
                    return View(newModel);
                }
            }
            return View(newModel);
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
            var rooms = _database.GetAllRooms();
            var newModel = new AddLightViewModel { Rooms = rooms.ToList() };
            if (ModelState.IsValid)
            {
                if (_database.AddLight(model))
                {
                    ViewBag.Message = "Šviestuvas pridėtas sėkmingai";
                    return View(newModel);
                }
            }
            return View(newModel);
        }

        [HttpPost]
        public async Task<IActionResult> UnblockLight([FromForm]Guid lightId, string triggerName, string roomName, int floorNumber)
        {
            var mqttTopic = $"{floorNumber}/{roomName}/{triggerName}";
            var light = _database.GetLightById(lightId);
            var lightPin = light.ControllerPin;
            var mqttMessage = $"Unblock;{lightPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
            mqttMessage = $"Off;{lightPin}";
            await _mqttClient.PublishMessageAsync(mqttTopic, mqttMessage);
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

        [HttpPost]
        public IActionResult AssignLightsToUser(AssignLightsToUserViewModel model)
        {
            foreach (var room in model.RoomIds)
            {
                var lights = _database.GetAllLightInRoom(room);
                _database.AssignLightsToUser(lights.ToList(), model.UserName);
            }
            TempData["Message"] = "Šviestuvai priskirti sėkmingai";
            return RedirectToAction("Index", "Administrator");
        }
    }
}
