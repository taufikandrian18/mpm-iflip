using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IInboxAttachmentAppService : IApplicationService
    {
        InboxAttachments GetById(Guid id);
        void Create(InboxAttachments input);
        void Update(InboxAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
