using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IAchievementsController : IApplicationService
    {
       BaseResponse GetAllBackoffice(Pagination request);
        Achievements GetByIDBackoffice(Guid guid);
        Achievements CreateBackoffice(Achievements model);
        Achievements UpdateBackoffice(Achievements model);
        String DestroyBackoffice(Guid guid);
    }
}