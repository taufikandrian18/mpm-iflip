using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesTalkAttachmentAppService : FLPAppServiceBase, ISalesTalkAttachmentAppService
    {
        private readonly IRepository<SalesTalkAttachments, Guid> _salesTalkAttachmentRepository;

        public SalesTalkAttachmentAppService(IRepository<SalesTalkAttachments, Guid> salesTalkAttachmentRepository)
        {
            _salesTalkAttachmentRepository = salesTalkAttachmentRepository;
        }
       
        public SalesTalkAttachments GetById(Guid id)
        {
            var salesTalkAttachment = _salesTalkAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return salesTalkAttachment;
        }

        public void Create(SalesTalkAttachments input)
        {
            _salesTalkAttachmentRepository.Insert(input);
        }

        public void Update(SalesTalkAttachments input)
        {
            _salesTalkAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var salesTalkAttachment = _salesTalkAttachmentRepository.FirstOrDefault(x => x.Id == id);
            salesTalkAttachment.DeleterUsername = username;
            salesTalkAttachment.DeletionTime = DateTime.Now;
            _salesTalkAttachmentRepository.Update(salesTalkAttachment);
        }
    }
}
