using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Models;
using Newtonsoft.Json;


namespace ValdymoSistema.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class OperatorController : Controller
    {
        private readonly IDatabaseController _database;

        public OperatorController(IDatabaseController database)
        {
            _database = database;
        }
        public IActionResult GetEnergyUsage()
        {
            ViewBag.Message = TempData["Message"];
            var model = new GetEnergyUsageViewModel
            {
                Rooms = _database.GetAllRooms().ToList(),
                DateFrom = new DateTime(2021, 05, 11, 8, 00, 00),
                DateTo = new DateTime(2021, 05, 14, 8, 00, 00)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EnergyUsage(GetEnergyUsageViewModel model)
        {
            var list = _database.GetEnergyUsage(model);
            if (list.Values.Count > 0)
            {
                var room = _database.GetRoom(model.RoomId);
                var totalEnergyUsed = new List<TotalEnergyUsedModel>();
                foreach (var light in list.Keys)
                {
                    var energyUsed = Math.Round(list[light].Sum(light => light.EnergyUsage), 4);
                    totalEnergyUsed.Add(new TotalEnergyUsedModel {
                        Light = light,
                        EnergyUsed = energyUsed
                    });
                }
                var totalEnergyJson = JsonConvert.SerializeObject(totalEnergyUsed);
                var energyUsageModel = new EnergyUsageViewModel
                {
                    Room = room,
                    OnOffEvents = list,
                    TotalEnergyUsedJson = totalEnergyJson,
                    DateFrom = model.DateFrom,
                    DateTo = model.DateTo
                };
                return View(energyUsageModel);
            }
            else
            {
                TempData["Message"] = "Pasirinktais laikais nėra duomenų";
                return RedirectToAction("GetEnergyUsage");
            }
            
        }

        public IActionResult EnergyUsage(EnergyUsageViewModel model)
        {
            return View(model);
        }
    }
}
