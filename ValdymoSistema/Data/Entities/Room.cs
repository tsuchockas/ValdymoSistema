using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Data.Entities
{
    public class Room
    {
        [Key]
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public int FloorNumber { get; set; }
        public List<Trigger> Triggers { get; set; }
    }
}
