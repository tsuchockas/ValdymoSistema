using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class BurntLightHistoryViewModel
    {
        public Dictionary<Light, List<LightEvent>> BurntLightEvents { get; set; }
        public Dictionary<Light, Room> LightRooms { get; set; }
    }
}
