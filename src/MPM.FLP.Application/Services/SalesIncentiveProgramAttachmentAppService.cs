using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesIncentiveProgramAttachmentAppService : FLPAppServiceBase, ISalesIncentiveProgramAttachmentAppService
    {
        private readonly IRepository<SalesIncentiveProgramAttachments, Guid> _salesIncentiveProgramAttachmentRepository;

        public SalesIncentiveProgramAttachmentAppService(IRepository<SalesIncentiveProgramAttachments, Guid> salesIncentiveProgramAttachmentRepository)
        {
            _salesIncentiveProgramAttachmentRepository = salesIncentiveProgramAttachmentRepository;
        }

        public SalesIncentiveProgramAttachments GetById(Guid id)
        {
            var salesIncentiveProgramAttachment = _salesIncentiveProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return salesIncentiveProgramAttachment;
        }

        public void Create(SalesIncentiveProgramAttachments input)
        {
            _salesIncentiveProgramAttachmentRepository.Insert(input);
        }

        public void Update(SalesIncentiveProgramAttachments input)
        {
            _salesIncentiveProgramAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var salesIncentiveProgramAttachment = _salesIncentiveProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            salesIncentiveProgramAttachment.DeleterUsername = username;
            salesIncentiveProgramAttachment.DeletionTime = DateTime.Now;
            _salesIncentiveProgramAttachmentRepository.Update(salesIncentiveProgramAttachment);
        }
    }
}
