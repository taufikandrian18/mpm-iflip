using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class RolePermissionsController : FLPAppServiceBase, IRolePermissionsController
    {
        private readonly RolePermissionsAppService _appService;
        private readonly PermissionsAppService _permissionService;

        public RolePermissionsController(
            RolePermissionsAppService appService,
            PermissionsAppService permissionService
        )
        {
            _appService = appService;
            _permissionService = permissionService;
        }

        [HttpGet("/api/services/app/backoffice/RolePermissions/getAllPermissions")]
        public List<TBPermissions> GetAllPermissionsByRole(int roleId)
        {
            List<string> permissionIds = _appService.GetAll()
                          .Where(y => y.RoleId == roleId)
                          .Select(x => x.PermissionId.ToString())
                          .ToList();

            return _permissionService.GetAll().Where(x => permissionIds.Contains(x.Id.ToString())).ToList();
        }

        [HttpPost("/api/services/app/backoffice/RolePermissions/create")]
        public async Task<RolePermissions> CreateBackoffice([FromForm]RolePermissions model)
        {
            model.Id = Guid.NewGuid();
            _appService.Create(model);
            return model;
        }


        [HttpDelete("/api/services/app/backoffice/RolePermissions/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.Delete(guid);
            return "Successfully deleted";
        }
    }
}