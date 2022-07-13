using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Roles.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Roles.Backoffice
{
    public class RolesController : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppController
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;

        public RolesController(IRepository<Role> repository, RoleManager roleManager, UserManager userManager)
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/Roles/getAll")]
        public async Task<ListResultDto<RoleListDto>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        [HttpGet("/api/services/app/backoffice/Roles/getAllPermissions")]
        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList()
            ));
        }

        [HttpPost("/api/services/app/backoffice/Roles/create")]
        public override async Task<RoleDto> Create([FromForm]CreateRoleDto input)
        {

            var role = ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            await _roleManager.CreateAsync(role);

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        [HttpPut("/api/services/app/backoffice/Roles/update")]
        public override async Task<RoleDto> Update([FromForm]RoleDto input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            ObjectMapper.Map(input, role);

            await _roleManager.UpdateAsync(role);

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        [HttpDelete("/api/services/app/backoffice/Roles/delete")]
        public async Task<string> Delete(string id)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(id);
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                await _userManager.RemoveFromRoleAsync(user, role.NormalizedName);
            }

            await _roleManager.DeleteAsync(role);
            return "Success";
        }
    }
}
