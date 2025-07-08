using System;

namespace RhyoliteOrbit.DomainServices.Payroll.PayCalendars.Dto;

public class PayCalendarDetailDto
{
    public Guid Id { get; set; }
    public Guid PayCalendarId { get; set; }
    public int Days { get; set; }
    public int Month { get; set; }
}