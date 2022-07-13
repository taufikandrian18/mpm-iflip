using Abp.Application.Services;
using MPM.FLP.Services.Dto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IInternalUsersController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        BaseResponse GetAllUsers(Pagination request);
        Task<InternalUserDto> GetByIDBackoffice(int guid);
        Task<InternalUserDto> Edit(InternalUserDto model);
        ActionResult ExportExcel();
    }
}