using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IServiceTalkFlyerAttachmentAppService : IApplicationService
    {
        IQueryable<ServiceTalkFlyerAttachments> GetAll(Guid id);
        ServiceTalkFlyerAttachments GetById(Guid id);
        void Create(ServiceTalkFlyerAttachments input);
        void Update(ServiceTalkFlyerAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
