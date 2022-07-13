using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MPM.FLP.Roles.Dto;

namespace MPM.FLP.Roles
{
    public interface IRoleAppController : IAsyncCrudAppService<RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<RoleListDto>> GetRolesAsync();
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
        Task<RoleDto> Create(CreateRoleDto input);
        Task<RoleDto> Update(RoleDto input);
        Task<string> Delete(string id);
    }
}
