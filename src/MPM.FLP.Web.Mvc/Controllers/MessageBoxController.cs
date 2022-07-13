using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class MessageBoxController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}