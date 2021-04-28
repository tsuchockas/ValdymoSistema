using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Controllers
{
    public class DatabaseController : IDatabaseController
    {
        private readonly ApplicationDbContext _context;

        public DatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ChangeLightState(Light light, Light.LightState lightState)
        {
            _context.Lights.Where(l => l.LightId == light.LightId).FirstOrDefault().CurrentState = lightState;
            _context.SaveChanges();
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _context.Rooms.OrderBy(r => r.RoomId).ToList();
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
            var user = _context.Users.Where(u => u.UserName.Equals(UserName)).FirstOrDefault();
            return _context.Lights.Where(l => l.Users.Contains(user)).ToList();
        }

        public Room GetRoomForTrigger(Trigger trigger)
        {
            return _context.Rooms.Where(r => r.Triggers.Contains(trigger)).FirstOrDefault();
        }

        public Trigger GetTriggerForLight(Light light)
        {
            return _context.Triggers.Where(t => t.Lights.Contains(light)).FirstOrDefault();
        }
    }
}
