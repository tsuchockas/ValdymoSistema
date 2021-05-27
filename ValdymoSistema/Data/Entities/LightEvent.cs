using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ValdymoSistema.Data.Entities.Light;

namespace ValdymoSistema.Data.Entities
{
    public class LightEvent
    {
        [Key]
        public Guid LightEventId { get; set; }
        public Light Light { get; set; }
        public LightState CurrentLightState { get; set; }
        public int Brightness { get; set; }
        public DateTime DateTime { get; set; }
        public double EnergyUsage { get; set; }


    }
}
