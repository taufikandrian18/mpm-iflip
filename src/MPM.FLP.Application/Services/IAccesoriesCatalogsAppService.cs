using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IAccesoriesCatalogAppService : IApplicationService
    {
        IQueryable<AccesoriesCatalogs> GetAll();
        List<Guid> GetAllIds();
        AccesoriesCatalogs GetById(Guid id);
        void Create(AccesoriesCatalogs input);
        void Update(AccesoriesCatalogs input);
        void SoftDelete(Guid Id, string username);
    }
}
