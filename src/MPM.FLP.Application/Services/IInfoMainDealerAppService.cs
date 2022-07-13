using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IInfoMainDealerAppService : IApplicationService
    {
        IQueryable<InfoMainDealers> GetAll();
        ICollection<InfoMainDealerAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds();
        InfoMainDealers GetById(Guid id);
        void Create(InfoMainDealers input);
        void Update(InfoMainDealers input);
        void SoftDelete(Guid id, string username);
    }
}
