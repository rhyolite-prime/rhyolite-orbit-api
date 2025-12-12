using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using RhyoliteOrbit.Controllers;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.Web.Controllers;

public class HomeController : RhyoliteOrbitControllerBase
{
    public async Task<ActionResult> StudentBillReport()
    {
        await Task.Delay(1);
       
        dynamic viewmodel = new ExpandoObject();

        // Initialize a sample StudentBillResult with test/mock data
        var result = new StudentBillDto
        {
            ReportHeader = new BillReportHeader
            {
                SchoolName = "Christ Royal International School",
                PrimaryPhoneNo = "0548043779",
                SchAddress = "123 Main St",
                LogoUrl = "https://res.cloudinary.com/rhyoliteprime/image/upload/v1673255393/school_manager_repository/cris-logo.jpg",
                Email = "Kwabenaf29@gmail.com"
            },
            StudentInfo = new StudentInfo
            {
                FullName = "Kwabena Addo",
                EnrollmentDate = "2021-09-01",
                StudentIdentifier = "ACM123456",
                Gender = "Male",
                ClassName = "Class 1",
                DateOfBirth = "11-05-1993",
            },
            StudentBill = new StudentBillDetail
            {
                BillNo = "CR1002BL",
                BillDate = DateTime.Now,
                Period = "1st Term, 2025/2026",
                CurrencyName = "Ghana Cedi",
                CurrencyCode = "GHS",
                BillAmount = 1000.00m,
                BillBalance = 1200.00m,
                BalanceBF = 200.00m,
                PaymentChannels = new List<string> { "Bank", "USSD" },
                Details = new List<BillItem>
                {
                    new BillItem
                    {
                        Id = Guid.NewGuid(),
                        FeeId = Guid.NewGuid(),
                        FeeName = "Tuition",
                        BillTypeId = Guid.NewGuid(),
                        BillTypeName = "Main Bill",
                        FeeAmount = 1000.00m,
                    },
                }
            }
        };

        viewmodel.StudentBillResult = result;

        return View(viewmodel);
    }
}
