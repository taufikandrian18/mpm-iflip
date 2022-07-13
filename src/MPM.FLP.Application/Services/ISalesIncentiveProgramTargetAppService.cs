using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ISalesIncentiveProgramTargetAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        SalesIncentiveProgramTarget GetById(Guid Id);
        void Create(SalesIncentiveProgramTargetCreateDto input);
        void Update(SalesIncentiveProgramTargetUpdateDto input);
        void SoftDelete(SalesIncentiveProgramTargetDeleteDto input);
    }
}
