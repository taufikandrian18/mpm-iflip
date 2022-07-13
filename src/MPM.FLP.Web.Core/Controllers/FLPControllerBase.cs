using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace MPM.FLP.Controllers
{
    public abstract class FLPControllerBase: AbpController
    {
        protected FLPControllerBase()
        {
            LocalizationSourceName = FLPConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
