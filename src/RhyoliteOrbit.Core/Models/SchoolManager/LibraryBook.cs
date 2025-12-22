using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class LibraryBook: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Isbn { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Category { get; set; } // e.g., Fiction, Non-Fiction, Science, History
    public Guid LibraryId { get; set; }
    public Guid LibraryName { get; set; }
    [Column(TypeName = "jsonb")] public List<string> Keywords { get; set; }
    public int TenantId { get; set; }
    
}
