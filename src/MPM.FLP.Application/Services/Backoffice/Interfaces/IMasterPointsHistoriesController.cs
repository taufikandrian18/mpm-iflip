using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface IMasterPointsHistoriesController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SPDCPointHistories GetByIDBackoffice(Guid guid);
        Task<String> Create(IEnumerable<IFormFile> files);
        SPDCPointHistories Edit(SPDCPointHistories model);
        String DestroyBackoffice(Guid guid);
    }
}
