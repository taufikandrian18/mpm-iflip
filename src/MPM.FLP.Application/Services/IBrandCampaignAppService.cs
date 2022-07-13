using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IBrandCampaignAppService : IApplicationService
    {
        IQueryable<BrandCampaigns> GetAll();
        ICollection<BrandCampaignAttachments> GetAllAttachments(Guid id);
        BrandCampaigns GetLast();
        BrandCampaigns GetById(Guid id);
        void Create(BrandCampaigns input);
        void Update(BrandCampaigns input);
        void SoftDelete(Guid id, string username);
    }
}
