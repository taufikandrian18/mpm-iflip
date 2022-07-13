using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IMotivationCardsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        MotivationCards GetByIDBackoffice(Guid guid);
        Task<MotivationCards> Create(MotivationCards model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images);
        Task<MotivationCards> Edit(MotivationCards model, IEnumerable<IFormFile> images);
        String Destroy(Guid guid);
    }
}