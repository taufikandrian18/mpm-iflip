using Abp.Application.Services;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IAttachmentFileAppService : IApplicationService
    {
        Task<string> GetAttachmentDownloadUrl(string url);
    }
}
