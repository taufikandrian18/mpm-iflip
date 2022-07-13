using Abp.AutoMapper;
using MPM.FLP.MPMWallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto.MpmWallet
{
    [AutoMap(typeof(MpmWalletHistoryRequest))]
    public class MpmWalletHistoryRequestDto
    {
        public string LastRefId { get; set; }
        public string CriteriaStartDate { get; set; }
        public string CriteriaEndDate { get; set; }
    }

    [AutoMap(typeof(MpmWalletHistoryResponse))]
    public class MpmWalletHistoryResponseDto : MpmWalletResponseDto
    {
        public string ClientId { get; set; }
        public string WalletId { get; set; }
        public string LastRefId { get; set; }
        public List<MpmWalletMutasi> Mutasi { get; set; }
        public static MpmWalletHistoryResponse NullWalletId()
        {
            return new MpmWalletHistoryResponse()
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
