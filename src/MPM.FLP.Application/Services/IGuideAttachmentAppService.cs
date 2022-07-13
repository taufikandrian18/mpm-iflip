using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IGuideAttachmentAppService : IApplicationService
    {
        GuideAttachments GetById(Guid id);
        void Create(GuideAttachments input);
        void Update(GuideAttachments input);
        void SoftDelete(Guid id, string username);
        Task<string> GetAttachmentDownloadUrl(Guid id);
    }
}
