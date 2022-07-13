using Abp.AutoMapper;
using MPM.FLP.MPMWallet;

namespace MPM.FLP.Services.Dto.MpmWallet
{
    [AutoMap(typeof(MpmWalletBalanceRequest))]
    public class MpmWalletBalanceRequestDto
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string AccountId { get; set; }
        public string Words { get; set; }
    }

    [AutoMap(typeof(MpmWalletBalanceResponse))]
    public class MpmWalletBalanceResponseDto : MpmWalletResponseDto
    {
        public string WalletId { get; set; }
        public decimal LastBalance { get; set; }
        public static MpmWalletBalanceResponse NullWalletId()
        {
            return new MpmWalletBalanceResponse()
            {
                ResponseCode = "2101",
                ResponseMessage = new MpmWalletResponseMessage()
                {
                    Id = "Akun pelanggan tidak ditemukan",
                    En = "Account recipient not found"
                }
            };
        }
    }
}
