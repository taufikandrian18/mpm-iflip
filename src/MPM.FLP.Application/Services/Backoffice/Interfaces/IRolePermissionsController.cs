using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Backoffice
{
    public interface IRolePermissionsController : IApplicationService
    {
        List<TBPermissions> GetAllPermissionsByRole(int roleId);
        Task<RolePermissions> CreateBackoffice(RolePermissions model);
        String DestroyBackoffice(Guid guid);
    }
}