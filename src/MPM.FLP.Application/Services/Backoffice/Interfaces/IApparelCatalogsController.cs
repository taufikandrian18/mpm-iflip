using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IApparelCatalogsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ApparelCatalogs GetByIDBackoffice(Guid guid);
        Task<ApparelCatalogs> CreateBackoffice(ApparelCatalogsVM model, IEnumerable<IFormFile> files);
        Task<ApparelCatalogs> UpdateBackoffice(ApparelCatalogsVM model, IEnumerable<IFormFile> files);
        String DestroyBackoffice(Guid guid);
    }
}