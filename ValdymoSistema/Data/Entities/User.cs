using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Data.Entities
{
    public class User : IdentityUser
    {
        public List<Light> Lights { get; set; }
        public string FullName { get; set; }
    }
}
