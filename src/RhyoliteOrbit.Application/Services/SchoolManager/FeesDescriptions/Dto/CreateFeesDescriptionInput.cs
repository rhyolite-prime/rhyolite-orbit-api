using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.FeesDescriptions.Dto;

public class CreateFeesDescriptionInput
{
    public string Description { get; set; }
    public Guid BillTypeId { get; set; }
        
    public Guid ReceivableAccountId { get; set; }
    public string ReceivableAccountName { get; set; }
    
    public string BillTypeName { get; set; }
}

public class UpdateFeesDescriptionInput: EntityDto<Guid>
{
    public string Description { get; set; }
    public Guid BillTypeId { get; set; }
        
    public Guid ReceivableAccountId { get; set; }
    public string ReceivableAccountName { get; set; }
    
    public string BillTypeName { get; set; }
}