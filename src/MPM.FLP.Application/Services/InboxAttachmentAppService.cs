using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class InboxAttachmentAppService : FLPAppServiceBase, IInboxAttachmentAppService
    {
        private readonly IRepository<InboxAttachments, Guid> _inboxAttachmentRepository;

        public InboxAttachmentAppService(IRepository<InboxAttachments, Guid> inboxAttachmentRepository)
        {
            _inboxAttachmentRepository = inboxAttachmentRepository;
        }

        public InboxAttachments GetById(Guid id)
        {
            var InboxAttachment = _inboxAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return InboxAttachment;
        }

        public void Create(InboxAttachments input)
        {
            _inboxAttachmentRepository.Insert(input);
        }

        public void Update(InboxAttachments input)
        {
            _inboxAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var InboxAttachment = _inboxAttachmentRepository.FirstOrDefault(x => x.Id == id);
            InboxAttachment.DeleterUsername = username;
            InboxAttachment.DeletionTime = DateTime.Now;
            _inboxAttachmentRepository.Update(InboxAttachment);
        }
    }
}
