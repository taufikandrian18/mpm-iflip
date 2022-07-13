using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Authorization.Users;
using MPM.FLP.MPMWallet;
using MPM.FLP.Services.Dto.MpmWallet;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IMpmWalletAppService : IApplicationService
    {
        Task<MpmWalletRegisterResponseDto> Register();
        Task<NewMpmRegisterResponse> NewRegister();
        Task<MpmWalletBalanceResponseDto> Balance();
        Task<MpmWalletHistoryResponseDto> History(string lastRefId, string criteriaStartDate, string criteriaEndDate);
        Task Callback(int idMpm, string accountId);

        Task<UserResponse> Check();
    }
}
