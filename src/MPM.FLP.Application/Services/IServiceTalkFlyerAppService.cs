using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IServiceTalkFlyerAppService : IApplicationService
    {
        IQueryable<ServiceTalkFlyers> GetAll();
        IQueryable<ServiceTalkFlyers> GetAllBackoffice();
        ICollection<ServiceTalkFlyerAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds();
        ServiceTalkFlyers GetById(Guid id);
        void Create(ServiceTalkFlyers input);
        void Update(ServiceTalkFlyers input);
        void SoftDelete(Guid id, string username);
    }
}
