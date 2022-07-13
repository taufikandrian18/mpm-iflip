using System.Collections.Generic;
using MPM.FLP.Roles.Dto;

namespace MPM.FLP.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}