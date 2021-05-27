using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class AssignLightsToUserViewModel
    {
        public List<User> RegisteredUsers { get; set; }
        public List<Room> RegisteredRooms { get; set; }
        [Display(Name = "Vartotojas")]
        public string UserName { get; set; }
        [Display(Name = "Patalpa")]
        [Required]
        public List<Guid> RoomIds { get; set; }

    }
}
