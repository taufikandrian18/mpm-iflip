using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface IMasterMenuPointController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        PointConfigurations GetByIDBackoffice(Guid guid);
        List<ItemDropDown> GetContentType();
        List<ItemDropDown> GetActivityType();
        Task<AddPointConfigurationDto> CreateDefault(AddPointConfigurationDto model);
        Task<UpdatePointConfigurationDto> Edit(UpdatePointConfigurationDto model);
        Task<String> DestroyBackoffice(Guid guid);

        List<PointConfigurations> Grid_Detail_Read(string content);
        Task<PointConfigurationDto> CreateDetailPoint(AddPointConfigurationDto model);
        Task<PointConfigurationDto> EditDetailPoint(PointConfigurationDto model);
    }
}
