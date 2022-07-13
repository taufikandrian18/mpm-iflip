using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IOnlinemagazinesController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        OnlineMagazines GetByIDBackoffice(Guid guid);
        Task<OnlineMagazines> Create(OnlineMagazines model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images);
        Task<OnlineMagazines> Edit(OnlineMagazines model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images);
        String Destroy(Guid guid);
    }
}