using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IKategoriPanduanLayananController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        GuideCategories GetByIDBackoffice(Guid guid);
        GuideCategories Create(GuideCategories model);
        GuideCategories Edit(GuideCategories model);
        String Destroy(Guid guid);
    }
}