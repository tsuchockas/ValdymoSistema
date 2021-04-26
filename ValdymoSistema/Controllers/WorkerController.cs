using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Services;
using ValdymoSistema.Services.Mqtt;

namespace ValdymoSistema.Controllers
{
    public class WorkerController : Controller
    {
        private MqttClient _mqttClient;
        public WorkerController(MqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }
        public async Task<IActionResult> Index()
        {
            await _mqttClient.PublishMessageAsync("Testing", "Made it to index page");
            return View();
        }
    }
}
