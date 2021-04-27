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

        public Light GetLightById(Guid lightId)
        {
            return _context.Lights.Where(l => l.LightId == lightId).FirstOrDefault();
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
