using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.MPMWallet
{
    public class MpmWalletBalanceRequest
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string AccountId { get; set; }
        public string Words { get; set; }
    }

    public class MpmWalletBalanceResponse : MpmWalletResponse
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
