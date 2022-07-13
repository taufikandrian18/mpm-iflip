using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IDealerH3AppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        DealerH3 GetById(string id);
        void Create(DealerH3 input);
        void Update(DealerH3 input);
        void SoftDelete(string id, string username);
        Task<ServiceResult> Sync();
    }
}
