using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Data.Entities;
using ValdymoSistema.Services;

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
            await _mqttClient.PublishMessageAsync("Testing", "Made it to index page");
            var currentUser = User.Identity.Name;
            var lights = _database.GetLightsForUser(currentUser);
            return View();
        }
    }
}
