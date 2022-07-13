using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IApplicationFeatureAppService : IApplicationService
    {
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        BaseResponse GetUser([FromQuery] Pagination request);
        ApplicationFeature GetById(Guid Id);
        void Create(ApplicationFeatureCreateDto input);
        void Update(ApplicationFeatureUpdateDto input);
        void UpdateMapping(ApplicationFeatureMappingUpdateDto input);
        void SoftDelete(ApplicationFeatureDeleteDto input);
    }
}
