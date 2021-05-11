using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class EnergyUsageViewModel
    {
        public Dictionary<Light, List<LightEvent>> OnOffEvents {get; set;}
        public Room Room { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Dictionary<Light, double> TotalEnergyUsed { get; set; }
    }
}
