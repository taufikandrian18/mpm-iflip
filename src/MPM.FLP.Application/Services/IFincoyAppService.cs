using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IFincoyAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        Fincoy GetById(Guid Id);
        void Create(FincoyCreateDto input);
        void Update(FincoyUpdateDto input);
        void SoftDelete(ProductTypesDeleteDto input);
    }
}
