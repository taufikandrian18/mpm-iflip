using MPM.FLP.MPMWallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Doku.Dto
{
    public class DokuBalanceRequestDto
    {
        public DokuBalanceRequestDto(DokuSettings dokuSettings, string accountId)
        {
            ClientId = dokuSettings.ClientId;
            AccessToken = DokuSettings.AccessToken;
            AccountId = accountId;
            Words = dokuSettings.GetHash(dokuSettings.ClientId + DokuSettings.Systrace + dokuSettings.SharedKey + accountId);
        }

        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string AccountId { get; set; }
        public string Words { get; set; }
    }
    public class DokuBalanceResponseDto : MpmWalletResponse
    {
        public string ClientId { get; set; }
        [Newtonsoft.Json.JsonProperty("dokuId")]
        public string WalletId { get; set; }
        public decimal LastBalance { get; set; }

        public MpmWalletBalanceResponse MapToMpmWallet()
        {
            MpmWalletBalanceResponse mpmWalletBalanceResponse = new MpmWalletBalanceResponse();
            mpmWalletBalanceResponse.WalletId = WalletId;
            mpmWalletBalanceResponse.LastBalance = LastBalance;
            mpmWalletBalanceResponse.ResponseCode = ResponseCode;
            mpmWalletBalanceResponse.ResponseMessage = ResponseMessage;

            return mpmWalletBalanceResponse;
        }
    }
}
