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
        public IEnumerable<Light> GetLightsForUser(Guid UserId)
        {
            throw new NotImplementedException();
        }
    }
}
