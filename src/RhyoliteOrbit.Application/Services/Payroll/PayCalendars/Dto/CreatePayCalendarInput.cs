using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.PayCalendars.Dto;

public class CreatePayCalendarInput
{
    public int Year { get; set; }
    public List<PayCalendarDetail> Details { get; set; }
}

public class UpdatePayCalendarInput:EntityDto<Guid>
{
    public int Year { get; set; }
    public List<PayCalendarDetail> Details { get; set; }
}