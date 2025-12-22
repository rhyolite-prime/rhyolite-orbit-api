using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BillSetupDetail
{
    public Guid Id { get; set; }
    public Guid FeeId { get; set; }
    public string FeeName { get; set; }
    public Guid BillTypeId { get; set; }
    
    public string Gender { get; set; } // Male, Female
    public string BillTypeName { get; set; }
    public decimal FeeAmount { get; set; }
}