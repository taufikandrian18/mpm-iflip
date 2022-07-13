using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IBASTReportAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        BASTReport GetById(Guid id);
        void Create(BASTReportCreateDto input);
        void Update(BASTReportUpdateDto input);
        void SoftDelete(Guid id);
    }
}
