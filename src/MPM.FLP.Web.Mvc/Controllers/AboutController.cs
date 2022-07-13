using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;

namespace MPM.FLP.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : FLPControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
