using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class GetEnergyUsageViewModel
    {
        [Required]
        [Display(Name = "Patalpa")]
        public Guid RoomId { get; set; }
        [Required]
        [Display(Name = "Data nuo")]
        public DateTime DateFrom { get; set; }
        [Required]
        [Display(Name = "Data iki")]
        public DateTime DateTo { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
