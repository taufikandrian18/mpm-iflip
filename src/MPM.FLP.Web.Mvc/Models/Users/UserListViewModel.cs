using System.Collections.Generic;
using MPM.FLP.Roles.Dto;
using MPM.FLP.Users.Dto;

namespace MPM.FLP.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
