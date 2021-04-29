using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class AddLightViewModel
    {
        public List<Room> Rooms { get; set; }
        [Required]
        [Display(Name = "Jungiklis")]
        public string TriggerId { get; set; }
        [Required(ErrorMessage = "Privalomas laukas")]
        [Range(0, 50, ErrorMessage = "Jungties skaičius turi būti tarp {1} ir {2}")]
        [Display(Name = "Valdiklio jungtis")]
        public int ControllerPin { get; set; }

        public string StatusMessage { get; set; }
    }
}
