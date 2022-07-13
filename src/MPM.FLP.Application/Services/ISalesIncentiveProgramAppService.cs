using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesIncentiveProgramAppService : IApplicationService
    {
        IQueryable<SalesIncentivePrograms> GetAll();
        SalesIncentivePrograms GetById(Guid id);
        List<Guid> AllIdByKotaJabatan(SalesIncentiveProgramGetIdDto input);
        void Create(SalesIncentivePrograms input);
        //void CreateIncentive(SalesIncentiveProgramsCreateDto input);
        void Update(SalesIncentivePrograms input);
        void SoftDelete(Guid id, string username);
        SalesIncentiveDashboardDto Dashboard(Guid SalesIncentiveId);
    }
}
