using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return Content("Icaze yoxdu");
        }
    }
}
