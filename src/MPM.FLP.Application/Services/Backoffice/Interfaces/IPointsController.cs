using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface IPointsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        PointConfigurations GetByIDBackoffice(Guid guid);
        AddPointConfigurationDto Create(AddPointConfigurationDto model);
        Task<UpdatePointConfigurationDto> Edit(UpdatePointConfigurationDto model);
        Task<String> Destroy(Guid guid);
    }
}