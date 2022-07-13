using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesProgramAttachmentAppService : FLPAppServiceBase, ISalesProgramAttachmentAppService
    {
        private readonly IRepository<SalesProgramAttachments, Guid> _salesProgramAttachmentRepository;

        public SalesProgramAttachmentAppService(IRepository<SalesProgramAttachments, Guid> salesProgramAttachmentRepository)
        {
            _salesProgramAttachmentRepository = salesProgramAttachmentRepository;
        }

        public SalesProgramAttachments GetById(Guid id)
        {
            var salesProgramAttachment = _salesProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return salesProgramAttachment;
        }

        public void Create(SalesProgramAttachments input)
        {
            _salesProgramAttachmentRepository.Insert(input);
        }

        public void Update(SalesProgramAttachments input)
        {
            _salesProgramAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var salesProgramAttachment = _salesProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            salesProgramAttachment.DeleterUsername = username;
            salesProgramAttachment.DeletionTime = DateTime.Now;
            _salesProgramAttachmentRepository.Update(salesProgramAttachment);
        }
    }
}
