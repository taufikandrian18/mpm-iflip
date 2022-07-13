using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISparepartCatalogAppService : IApplicationService
    {
        IQueryable<SparepartCatalogs> GetAll();
        List<Guid> GetAllIds();
        SparepartCatalogs GetById(Guid id);
        void Create(SparepartCatalogs input);
        void Update(SparepartCatalogs input);
        void SoftDelete(Guid Id, string username);
    }
}
