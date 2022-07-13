using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IBrandCampaignAttachmentAppService : IApplicationService
    {
        BrandCampaignAttachments GetById(Guid id);
        void Create(BrandCampaignAttachments input);
        void Update(BrandCampaignAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
