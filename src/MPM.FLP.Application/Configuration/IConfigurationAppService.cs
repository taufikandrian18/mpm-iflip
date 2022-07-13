using System.Threading.Tasks;
using MPM.FLP.Configuration.Dto;

namespace MPM.FLP.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
