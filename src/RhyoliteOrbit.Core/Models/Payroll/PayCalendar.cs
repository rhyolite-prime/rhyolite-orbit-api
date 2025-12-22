using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class PayCalendar: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public int Year { get; set; }
    [Column(TypeName = "jsonb")] public List<PayCalendarDetail> Details { get; set; }
    public int TenantId { get; set; }
}