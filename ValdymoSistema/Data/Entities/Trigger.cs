using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Data.Entities
{
    public class Trigger
    {
        [Key]
        public Guid TriggerId { get; set; }
        public string TriggerName { get; set; }
        public List<Light> Lights { get; set; }
    }
}
