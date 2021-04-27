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
        public IEnumerable<Light> GetLightsForUser(string UserName)
        {
            var user = _context.Users.Where(u => u.UserName.Equals(UserName)).FirstOrDefault();
            return user.Lights.ToList();
        }
    }
}
