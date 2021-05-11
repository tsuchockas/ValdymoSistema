using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Models;

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
                var energyUsageModel = new EnergyUsageViewModel
                {
                    Room = room,
                    OnOffEvents = list
                };
                //foreach (var key in list.Keys)
                //{
                //    foreach (var item in list[key])
                //    {
                //        var hoursLit = Math.Round((item.Value.DateTime - item.Key.DateTime).TotalHours, 2);
                //        var energyUsed = Math.Round(item.Key.EnergyUsage * hoursLit, 2);
                //        //Console.WriteLine(energyUsed);
                //    }
                //}
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
