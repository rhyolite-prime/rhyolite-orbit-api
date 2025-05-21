using RhyoliteOrbit.Roles.Dto;
using System.Collections.Generic;

namespace RhyoliteOrbit.Web.Models.Common;

public interface IPermissionsEditViewModel
{
    List<FlatPermissionDto> Permissions { get; set; }
}