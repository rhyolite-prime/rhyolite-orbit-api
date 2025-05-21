using RhyoliteOrbit.Roles.Dto;
using System.Collections.Generic;

namespace RhyoliteOrbit.Web.Models.Users;

public class UserListViewModel
{
    public IReadOnlyList<RoleDto> Roles { get; set; }
}
