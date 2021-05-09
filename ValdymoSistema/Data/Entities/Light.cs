using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Data.Entities
{
    public class Light
    {
        public enum LightState
        {
            On,
            Off,
            Blocked,
            Burnt
        }
        [Key]
        public Guid LightId { get; set; }
        public int ControllerPin { get; set; }
        public LightState CurrentState { get; set; }
        public List<User> Users { get; set; }
        public int CurrentBrightness { get; set; }
    }
}
