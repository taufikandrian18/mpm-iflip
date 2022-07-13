using System.Threading.Tasks;
using Abp.Application.Services;
using MPM.FLP.Sessions.Dto;

namespace MPM.FLP.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
