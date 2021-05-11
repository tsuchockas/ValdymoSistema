using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Models
{
    public class TotalEnergyUsedModel
    {
        public Light Light { get; set; }
        public double EnergyUsed { get; set; }
    }
}
