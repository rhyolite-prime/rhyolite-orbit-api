using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.DocumentManagement;

public class Document : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required]
    [MaxLength(350)]
    public string FileUrl { get; set; }
    
    [MaxLength(150)]
    public string FileName { get; set; }
    
    [MaxLength(50)]
    public string FileType { get; set; }
    
    public long FileSize { get; set; }
    
    [MaxLength(50)]
    public string Category { get; set; }
    
    public int TenantId { get; set; }
}
