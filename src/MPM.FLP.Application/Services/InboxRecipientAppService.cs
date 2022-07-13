using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class InboxRecipientAppService : FLPAppServiceBase, IInboxRecipientAppService
    {
        private readonly IRepository<InboxRecipients, Guid> _inboxRecipientRepository;

        public InboxRecipientAppService(IRepository<InboxRecipients, Guid> inboxRecipientRepository)
        {
            _inboxRecipientRepository = inboxRecipientRepository;
        }

        public IQueryable<InboxRecipients> GetAll()
        {
            return _inboxRecipientRepository.GetAll().Include(y => y.InternalUser).Include(y => y.InboxMessages);
        }

        public List<InboxRecipients> GetByInboxMessage(Guid inboxMessageId)
        {
            var InboxRecipients = _inboxRecipientRepository.GetAllIncluding(x => x.InternalUser)
                    .Where(x => x.InboxMessageId == inboxMessageId).ToList();
            return InboxRecipients;
        }

        public InboxRecipients GetById(Guid id)
        {
            var InboxRecipient = _inboxRecipientRepository.FirstOrDefault(x => x.Id == id);
            return InboxRecipient;
        }

        public List<InboxRecipients> GetByUser(int idMPM)
        {
            var InboxRecipients = _inboxRecipientRepository.GetAllIncluding(x => x.InternalUser).OrderByDescending(x => x.CreationTime)
                    .Where(x => x.IDMPM == idMPM && string.IsNullOrEmpty(x.InboxMessages.DeleterUsername)).ToList();
            return InboxRecipients;
        }

        public void Create(InboxRecipients input)
        {
            _inboxRecipientRepository.Insert(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var InboxRecipient = _inboxRecipientRepository.FirstOrDefault(x => x.Id == id);
            InboxRecipient.DeleterUsername = username;
            InboxRecipient.DeletionTime = DateTime.Now;
            _inboxRecipientRepository.Update(InboxRecipient);
        }
    }
}
