using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.Authorization;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class BrandCampaignAttachmentAppService : FLPAppServiceBase, IBrandCampaignAttachmentAppService
    {
        private readonly IRepository<BrandCampaignAttachments, Guid> _brandCampaignAttachmentRepository;

        public BrandCampaignAttachmentAppService(IRepository<BrandCampaignAttachments, Guid> brandCampaignAttachmentRepository)
        {
            _brandCampaignAttachmentRepository = brandCampaignAttachmentRepository;
        }

        public BrandCampaignAttachments GetById(Guid id)
        {
            var BrandCampaignAttachment = _brandCampaignAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return BrandCampaignAttachment;
        }

        public void Create(BrandCampaignAttachments input)
        {
            _brandCampaignAttachmentRepository.Insert(input);
        }

        public void Update(BrandCampaignAttachments input)
        {
            _brandCampaignAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var BrandCampaignAttachment = _brandCampaignAttachmentRepository.FirstOrDefault(x => x.Id == id);
            BrandCampaignAttachment.DeleterUsername = username;
            BrandCampaignAttachment.DeletionTime = DateTime.Now;
            _brandCampaignAttachmentRepository.Update(BrandCampaignAttachment);
        }
    }
}
