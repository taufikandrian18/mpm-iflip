using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesProgramAppService : IApplicationService
    {
        IQueryable<SalesPrograms> GetAll();
        IQueryable<SalesPrograms> GetAllBackoffice();
        ICollection<SalesProgramAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds(string channel);
        SalesPrograms GetById(Guid id);
        void Create(SalesPrograms input);
        void Update(SalesPrograms input);
        void SoftDelete(Guid id, string username);
    }
}
