using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IPermissionsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        TBPermissions GetByIDBackoffice(Guid guid);
        Task<TBPermissions> CreateBackoffice(PermissionsVM model);
        Task<TBPermissions> EditBackoffice(PermissionsVM model);
        String DestroyBackoffice(Guid guid);
    }
}