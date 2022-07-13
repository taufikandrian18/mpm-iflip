using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IInboxRecipientAppService : IApplicationService
    {
        IQueryable<InboxRecipients> GetAll();
        InboxRecipients GetById(Guid id);
        List<InboxRecipients> GetByUser(int idMPM);
        List<InboxRecipients> GetByInboxMessage(Guid inboxMessageId);
        void Create(InboxRecipients input);
        void SoftDelete(Guid input, string username);
    }
}
