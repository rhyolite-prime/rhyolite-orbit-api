using Abp.AutoMapper;
using RhyoliteOrbit.Roles.Dto;
using RhyoliteOrbit.Web.Models.Common;

namespace RhyoliteOrbit.Web.Models.Roles;

[AutoMapFrom(typeof(GetRoleForEditOutput))]
public class EditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
{
    public bool HasPermission(FlatPermissionDto permission)
    {
        return GrantedPermissionNames.Contains(permission.Name);
    }
}
