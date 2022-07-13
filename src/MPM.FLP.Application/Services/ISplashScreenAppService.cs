using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ISplashScreenAppService : IApplicationService
    {
        BaseResponse GetUser([FromQuery] Pagination request);
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        void Create(SplashScreenCreateDto input);
        void Update(SplashScreenUpdateDto input);
        void SoftDelete(SplashScreenDeleteDto input);
    }
}
