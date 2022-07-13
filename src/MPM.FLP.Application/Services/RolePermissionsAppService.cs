using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services
{
    public class RolePermissionsAppService : FLPAppServiceBase, IRolePermissionsAppService
    {
        private readonly IRepository<RolePermissions, Guid> _rolePermissionRepository;
        public RolePermissionsAppService(IRepository<RolePermissions, Guid> rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public IQueryable<RolePermissions> GetAll()
        {
            return _rolePermissionRepository.GetAll();
        }

        public void Create(RolePermissions input)
        {
            _rolePermissionRepository.InsertAndGetId(input);
        }

        public void Delete(Guid id)
        {
            var rolePermission = _rolePermissionRepository.GetAll().FirstOrDefault(x => x.Id == id);
            _rolePermissionRepository.Delete(rolePermission);
        }
    }
}
