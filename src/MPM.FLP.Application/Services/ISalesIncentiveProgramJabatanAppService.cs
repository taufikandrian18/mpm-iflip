using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesIncentiveProgramJabatanAppService : IApplicationService
    {
        IQueryable<SalesIncentiveProgramJabatans> GetAll();
        SalesIncentiveProgramJabatans GetById(Guid id);
        void Create(SalesIncentiveProgramJabatans input);
        void Delete(Guid id);
    }
}
