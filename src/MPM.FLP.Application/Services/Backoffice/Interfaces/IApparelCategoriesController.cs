using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IApparelCategoriesController : IApplicationService
    {
       BaseResponse GetAllBackoffice(Pagination request);
        ApparelCategories GetByIDBackoffice(Guid guid);
        ApparelCategories CreateBackoffice(ApparelCategoriesVM model);
        ApparelCategories UpdateBackoffice(ApparelCategoriesVM model);
        String DestroyBackoffice(Guid guid);
    }
}