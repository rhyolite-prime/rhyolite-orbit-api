using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsDocument: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }  
    public Guid FolderId { get; set; }
    [MaxLength(100)]
    public string FolderName { get; set; }
    [MaxLength(255)]
    public string FileName { get; set; }
    [MaxLength(500)]
    public string Title { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    public int FileSize { get; set; }
    public int Status { get; set; }
    public bool IsStarred { get; set; }
    [MaxLength(500)]
    public string MimeType { get; set; }
    [MaxLength(500)]
    public string FilePath { get; set; }
    [MaxLength(64)]
    public string Checksum { get; set; }
    [MaxLength(64)]
    public string VersionId { get; set; }
    public int TenantId { get; set; }
}