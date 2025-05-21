using System;

namespace RhyoliteOrbit.Models.Shared;

public class BankBranch
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
}