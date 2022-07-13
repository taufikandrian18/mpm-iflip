using MPM.FLP.MPMWallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Doku.Dto
{
    public class DokuHistoryRequestDto
    {
        public DokuHistoryRequestDto(DokuSettings dokuSettings, string accountId, string lastRefId, string criteriaStartDate, string criteriaEndDate)
        {
            ClientId = dokuSettings.ClientId;
            AccessToken = DokuSettings.AccessToken;
            AccountId = accountId;
            Words = dokuSettings.GetHash(dokuSettings.ClientId + DokuSettings.Systrace + dokuSettings.SharedKey + accountId);

            LastRefId = lastRefId;
            CriteriaStartDate = criteriaStartDate;
            CriteriaEndDate = criteriaEndDate;
        }

        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string AccountId { get; set; }
        public string Words { get; set; }
        public string LastRefId { get; set; }
        public string CriteriaStartDate { get; set; }
        public string CriteriaEndDate { get; set; }
        public string Version { get; set; } = "1.0";
    }

    public class DokuHistoryResponseDto : MpmWalletResponse
    {
        public string ClientId { get; set; }
        [Newtonsoft.Json.JsonProperty("dokuId")]
        public string WalletId { get; set; }
        public string LastRefId { get; set; }
        public List<DokuMutasi> Mutasi { get; set; }

        public MpmWalletHistoryResponse MapToMpmWallet()
        {
            MpmWalletHistoryResponse mpmWalletBalanceResponse = new MpmWalletHistoryResponse();
            mpmWalletBalanceResponse.WalletId = WalletId;
            mpmWalletBalanceResponse.LastRefId = LastRefId;
            mpmWalletBalanceResponse.ResponseCode = ResponseCode;
            mpmWalletBalanceResponse.ResponseMessage = ResponseMessage;
            mpmWalletBalanceResponse.Mutasi = new List<MpmWalletMutasi>();

            foreach (DokuMutasi dokuMutasi in Mutasi)
            {
                MpmWalletMutasi mpmWalletMutasi = new MpmWalletMutasi()
                {
                    RefId = dokuMutasi.RefId,
                    Type = dokuMutasi.Type,
                    Amount = dokuMutasi.Amount,
                    TransactionId = dokuMutasi.TransactionId,
                    Title = dokuMutasi.Title
                };
                mpmWalletBalanceResponse.Mutasi.Add(mpmWalletMutasi);
            }

            return mpmWalletBalanceResponse;
        }
    }

    public class DokuMutasi
    {
        public string RefId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public MpmWalletResponseMessage Title { get; set; }
        public string TransactionId { get; set; }
    }
}
