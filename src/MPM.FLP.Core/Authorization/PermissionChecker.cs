using Abp.Authorization;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
