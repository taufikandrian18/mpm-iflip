using Microsoft.AspNetCore.Antiforgery;
using MPM.FLP.Controllers;

namespace MPM.FLP.Web.Host.Controllers
{
    public class AntiForgeryController : FLPControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
