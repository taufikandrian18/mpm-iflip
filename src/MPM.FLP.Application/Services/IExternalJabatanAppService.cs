using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IExternalJabatanAppService : IApplicationService
    {
        IQueryable<ExternalJabatans> GetAll();
        List<ExternalJabatans> GetList();
        ExternalJabatans GetById(Guid id);
        void Create(ExternalJabatans input);
        void Update(ExternalJabatans input);
        void SoftDelete(Guid id, string username);
    }
}
