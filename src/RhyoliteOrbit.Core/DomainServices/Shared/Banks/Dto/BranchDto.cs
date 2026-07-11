using System;

namespace RhyoliteOrbit.DomainServices.Shared.Banks.Dto;

public class BranchDto
{
    public Guid BankId { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
}