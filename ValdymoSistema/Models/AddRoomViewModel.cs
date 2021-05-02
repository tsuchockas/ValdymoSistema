using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Models
{
    public class AddRoomViewModel
    {
        [Required(ErrorMessage = "Privalomas laukas")]
        [Display(Name = "Aukštas")]
        [Range(1, 20, ErrorMessage = "Privaloma pasirinkti skaičių tarp {1} ir {2}")]
        public int Floor { get; set; }
        [Required(ErrorMessage = "Privalomas laukas")]
        [Display(Name = "Patalpos pavadinimas")]
        public string RoomName { get; set; }
        public string StatusMessage { get; set; }
    }
}
