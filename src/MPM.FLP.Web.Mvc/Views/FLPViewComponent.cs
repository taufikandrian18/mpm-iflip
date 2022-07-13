using Abp.AspNetCore.Mvc.ViewComponents;

namespace MPM.FLP.Web.Views
{
    public abstract class FLPViewComponent : AbpViewComponent
    {
        protected FLPViewComponent()
        {
            LocalizationSourceName = FLPConsts.LocalizationSourceName;
        }
    }
}
