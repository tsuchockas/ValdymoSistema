using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Controllers
{
    public class AdministratorController : Controller
    {
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
