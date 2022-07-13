using System.Threading.Tasks;
using Abp.Application.Services;
using MPM.FLP.Authorization.Accounts.Dto;

namespace MPM.FLP.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        //Task<LoginOutput> Login(LoginInput input);
    }
}
