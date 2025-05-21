using System;

namespace RhyoliteOrbit.Models.Shared;

public class SupplierContact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OtherName { get; set; }
    public string PhoneNo { get; set; }
    public string EmailAddress { get; set; }
    public string BranchName { get; set; }
}