using Abp.AspNetCore.Mvc.Authorization;
using RhyoliteOrbit.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace RhyoliteOrbit.Web.Controllers;

[AbpMvcAuthorize]
public class HomeController : RhyoliteOrbitControllerBase
{
    public ActionResult Index()
    {
        return View();
    }
}
