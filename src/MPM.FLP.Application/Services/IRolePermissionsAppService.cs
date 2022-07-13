using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services
{
    public interface IRolePermissionsAppService : IApplicationService
    {
        IQueryable<RolePermissions> GetAll();
        void Create(RolePermissions input);
        void Delete(Guid id);
    }
}
