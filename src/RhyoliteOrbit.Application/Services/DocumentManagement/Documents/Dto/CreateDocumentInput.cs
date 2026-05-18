using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Documents.Dto;

public class CreateDocumentInput
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
}

public class UpdateDocumentInput : EntityDto<Guid>
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
}
