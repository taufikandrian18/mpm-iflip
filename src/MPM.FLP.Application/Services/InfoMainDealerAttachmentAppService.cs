using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class InfoMainDealerAttachmentAppService : FLPAppServiceBase, IInfoMainDealerAttachmentAppService
    {
        private readonly IRepository<InfoMainDealerAttachments, Guid> _infoMainDealerAttachmentRepository;

        public InfoMainDealerAttachmentAppService(IRepository<InfoMainDealerAttachments, Guid> infoMainDealerAttachmentRepository)
        {
            _infoMainDealerAttachmentRepository = infoMainDealerAttachmentRepository;
        }

        public InfoMainDealerAttachments GetById(Guid id)
        {
            var infoMainDealerAttachment = _infoMainDealerAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return infoMainDealerAttachment;
        }

        public void Create(InfoMainDealerAttachments input)
        {
            _infoMainDealerAttachmentRepository.Insert(input);
        }

        public void Update(InfoMainDealerAttachments input)
        {
            _infoMainDealerAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var infoMainDealerAttachment = _infoMainDealerAttachmentRepository.FirstOrDefault(x => x.Id == id);
            infoMainDealerAttachment.DeleterUsername = username;
            infoMainDealerAttachment.DeletionTime = DateTime.Now;
            _infoMainDealerAttachmentRepository.Update(infoMainDealerAttachment);
        }
    }
}
