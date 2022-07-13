using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.MPMWallet
{
    public class MpmWalletResponse
    {
        public string ResponseCode { get; set; }
        public MpmWalletResponseMessage ResponseMessage { get; set; }
    }

    public class MpmWalletResponseMessage
    {
        public string Id { get; set; }
        public string En { get; set; }
    }
}
