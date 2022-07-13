using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Roles;
using MPM.FLP.Roles.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class RolesController : FLPAppServiceBase, IRolesController
    {
        private readonly RoleAppService _roleAppService;
        private readonly RoleManager _roleManager;

        public RolesController(RoleAppService roleAppService, RoleManager roleManager)
        {
            _roleAppService = roleAppService;
            _roleManager = roleManager;
        }

        [HttpGet("/api/services/app/backoffice/Roles/getAllBackup")]
        public async Task<RoleListViewModel> Index()
        {
            var roles = (await _roleAppService.GetRolesAsync(new GetRolesInput())).Items;
            var permissions = (await _roleAppService.GetAllPermissions()).Items;
            var model = new RoleListViewModel
            {
                Roles = roles,
                Permissions = permissions
            };

            return model;
        }

        /*
        public async Task<ListResultDto<RoleListDto>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }
        */
    }
}
