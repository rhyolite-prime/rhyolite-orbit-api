using RhyoliteOrbit.Roles.Dto;
using System.Collections.Generic;

namespace RhyoliteOrbit.Web.Models.Roles;

public class RoleListViewModel
{
    public IReadOnlyList<PermissionDto> Permissions { get; set; }
}
