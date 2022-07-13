using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IExternalJabatanController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ExternalJabatans GetByIDBackoffice(Guid guid);
        Task<ExternalJabatans> Create(ExternalJabatans model);
        ExternalJabatans Edit(ExternalJabatans model);
        String DestroyBackoffice(Guid guid);
    }
}