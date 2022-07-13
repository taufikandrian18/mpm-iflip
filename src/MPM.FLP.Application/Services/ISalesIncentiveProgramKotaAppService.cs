using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesIncentiveProgramKotaAppService : IApplicationService
    {
        IQueryable<SalesIncentiveProgramKotas> GetAll();
        SalesIncentiveProgramKotas GetById(Guid id);
        void Create(SalesIncentiveProgramKotas input);
        void Delete(Guid id);
    }
}
