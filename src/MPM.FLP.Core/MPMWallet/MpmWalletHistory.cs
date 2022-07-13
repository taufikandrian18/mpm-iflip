using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.MPMWallet
{
    public class MpmWalletHistoryRequest
    {
        public string LastRefId { get; set; }
        public string CriteriaStartDate { get; set; }
        public string CriteriaEndDate { get; set; }
    }

    public class MpmWalletHistoryResponse : MpmWalletResponse
    {
        public string WalletId { get; set; }
        public string LastRefId { get; set; }
        public List<MpmWalletMutasi> Mutasi { get; set; }
    }

    public class MpmWalletMutasi
    {
        public string RefId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public MpmWalletResponseMessage Title { get; set; }
        public string TransactionId { get; set; }
    }
}
