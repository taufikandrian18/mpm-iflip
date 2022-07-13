using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IInboxMessageAppService : IApplicationService
    {
        IQueryable<InboxMessages> GetAll();
        ICollection<InboxAttachments> GetAllAttachments(Guid id);
        //List<Guid> GetAllIds();
        InboxMessages GetById(Guid id);
        void Create(InboxMessages input);
        void Update(InboxMessages input);
        void SoftDelete(Guid id, string username);
    }
}
