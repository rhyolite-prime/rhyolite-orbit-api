using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Sibling
{
    public Guid SiblingClassId { get; set; }
    [MaxLength(50)]
    public string SiblingClass { get; set; }
    public Guid SiblingId { get; set; }
    [MaxLength(20)]
    public string SiblingIdentifier { get; set; }
    [MaxLength(120)]
    public string SiblingFullName { get; set; }
    
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(120)]
    public string StudentName { get; set; }
    public int TenantId { get; set; }
}