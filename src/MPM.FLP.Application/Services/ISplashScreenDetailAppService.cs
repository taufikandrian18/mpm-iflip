using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ISplashScreenDetailAppService : IApplicationService
    {
        BaseResponse GetUser([FromQuery] Pagination request);
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        void Create(SplashScreenDetailsCreateDto input);
        void Update(SplashScreenDetailsUpdateDto input);
        void SoftDelete(SplashScreenDeleteDto input);
    }
}
