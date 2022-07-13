using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IAccesoriesCatalogsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        AccesoriesCatalogs GetByIDBackoffice(Guid guid);
        Task<AccesoriesCatalogs> CreateBackoffice(AccesoriesCatalogs model, IEnumerable<IFormFile> files);
        Task<AccesoriesCatalogs> UpdateBackoffice(AccesoriesCatalogs model, IEnumerable<IFormFile> files);
        String DestroyBackoffice(Guid guid);
    }
}