using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class LightsViewModel
    {
        public List<Light> Lights { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Trigger> Triggers { get; set; }
    }
}
