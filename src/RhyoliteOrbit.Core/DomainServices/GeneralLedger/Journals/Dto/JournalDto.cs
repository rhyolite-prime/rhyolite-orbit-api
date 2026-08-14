using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.Journals.Dto;

public class JournalDto
{
    public string BatchNumber { get; set; }
    public DateTime BatchDate { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal TotalDebit { get; set; }
    public bool IsMultiCurrency { get; set; }
    
    public Guid OuId { get; set; }
    
    public string OuName { get; set; }
    public List<JournalDetail> Details { get; set; }
    
}