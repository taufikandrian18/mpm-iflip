using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MPM.FLP.Roles.Dto;
using MPM.FLP.Users.Dto;

namespace MPM.FLP.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
        Task<UserDto> UpdateWallet(UserDto input);
    }
}
