using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class LaboratoryEquipment
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public int Quantity { get; set; }
    public string Condition { get; set; }
}