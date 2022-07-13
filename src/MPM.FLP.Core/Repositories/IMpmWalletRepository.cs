using MPM.FLP.MPMWallet;
using System.Threading.Tasks;

namespace MPM.FLP.Repositories
{
    public interface IMpmWalletRepository
    {
        Task<MpmWalletSignOnResponse> SignOn();
        Task<MpmWalletRegisterResponse> Register(MpmWalletRegisterRequest mpmWalletRegisterRequest);
        Task<NewMpmRegisterResponse> NewRegister(int idmpm);
        Task<NewMpmRegisterResponse> CheckToken(int idmpm);
        Task<MpmWalletBalanceResponse> Balance(string userId, string walletId);
        Task<MpmWalletHistoryResponse> History(string userId, string walletId, string lastRefId, string criteriaStartDate, string criteriaEndDate);
        string GetDokuSetting();
    }
}
