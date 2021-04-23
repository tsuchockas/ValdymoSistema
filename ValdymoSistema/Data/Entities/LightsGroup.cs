using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Data.Entities
{
    public class LightsGroup
    {
        [Key]
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Light> LightsInGroup { get; set; }
    }
}
