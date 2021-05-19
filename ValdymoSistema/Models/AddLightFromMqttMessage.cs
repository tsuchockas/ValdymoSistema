using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Models
{
    public class AddLightFromMqttMessage
    {
        public int FloorNumber { get; set; }
        public string RoomName { get; set; }
        public string TriggerName { get; set; }
        public int LightPin { get; set; }
    }
}
