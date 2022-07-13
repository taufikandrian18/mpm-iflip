using Abp.AutoMapper;
using MPM.FLP.MPMWallet;

namespace MPM.FLP.Services.Dto.MpmWallet
{
    [AutoMap(typeof(MpmWalletResponse))]
    public class MpmWalletResponseDto
    {
        public string ResponseCode { get; set; }
        public MpmWalletResponseMessageDto ResponseMessage { get; set; }
    }

    [AutoMap(typeof(MpmWalletResponseMessage))]
    public class MpmWalletResponseMessageDto
    {
        public string Id { get; set; }
        public string En { get; set; }
    }
}
