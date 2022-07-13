
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class DashboardController : FLPControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}