using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class AddTriggerViewModel
    {
        public List<Room> Rooms { get; set; }
        [Required(ErrorMessage = "Privalomas laukas")]
        [Display(Name = "Patalpa")]
        public string RoomId { get; set; }
        [Required(ErrorMessage = "Privalomas laukas")]
        [Display(Name = "Valdiklio pavadinimas")]
        public string ControllerName { get; set; }
        public string StatusMessage { get; set; }
    }
}
