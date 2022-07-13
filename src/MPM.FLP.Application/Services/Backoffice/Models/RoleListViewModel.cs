using System.Collections.Generic;
using MPM.FLP.Roles.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleListDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
