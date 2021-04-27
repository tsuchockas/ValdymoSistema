using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Models
{
    public class TurnOnLightModel
    {
        public int FloorNumber { get; set; }
        public string RoomName { get; set; }
        public string TriggerName { get; set; }
    }
}
