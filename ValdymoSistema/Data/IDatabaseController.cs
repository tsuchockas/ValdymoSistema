using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Data
{
    public interface IDatabaseController
    {
        IEnumerable<Light> GetLightsForUser(string UserName);
    }
}
