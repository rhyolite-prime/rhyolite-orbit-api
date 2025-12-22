using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using RhyoliteOrbit.Controllers;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.Web.Controllers;

public class HomeController : RhyoliteOrbitControllerBase
{
    public async Task<ActionResult> StudentBillReport(StudentBillDto dto)
    {
        await Task.Delay(1);
       
        dynamic viewmodel = new ExpandoObject();

        viewmodel.StudentBillDto = dto;

        return View(viewmodel);
    }
}
