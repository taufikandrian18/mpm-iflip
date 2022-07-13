using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using MPM.FLP.Configuration.Dto;

namespace MPM.FLP.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : FLPAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
