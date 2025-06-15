using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BillDetail
{
    public Guid Id { get; set; }
    public Guid FeeId { get; set; }
    public string FeeName { get; set; }
    public Guid BillTypeId { get; set; }
    public string BillTypeName { get; set; }
    public decimal FeeAmount { get; set; }
}