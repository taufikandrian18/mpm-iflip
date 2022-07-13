using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using System;

namespace MPM.FLP.Services
{
    public interface ISekolahAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        Sekolahs GetById(Guid id);
        void Create(Sekolahs input);
        void Update(Sekolahs input);
        void SoftDelete(Guid id, string username);
    }
}
