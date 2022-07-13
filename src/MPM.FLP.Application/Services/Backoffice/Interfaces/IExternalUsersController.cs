using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using System;
using MPM.FLP.Authorization.Users;
using Microsoft.AspNetCore.Http;
using MPM.FLP.Services.Dto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IExternalUsersController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        Task<ExternalUserDto> GetByIDBackoffice(Guid guid);
        Task<ExternalUserDto> EditExternal(ExternalUserDto model);
        ActionResult ExportExcel();
    }
}