using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace MPM.FLP.Web.Views
{
    public abstract class FLPRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected FLPRazorPage()
        {
            LocalizationSourceName = FLPConsts.LocalizationSourceName;
        }
    }
}
