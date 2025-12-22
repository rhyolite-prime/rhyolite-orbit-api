using System;

public class SubjectCategory : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; } // sciences, arts, languages, etc
    public string Description { get; set; }
    public int TenantId { get; set; }
}