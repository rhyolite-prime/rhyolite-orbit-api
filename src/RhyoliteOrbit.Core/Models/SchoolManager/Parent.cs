using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Parent: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string FirstGuardianName { get; set; }
    public string FirstGuardianPhoneNo { get; set; }
    public string FirstGuardianEmail { get; set; }
    public string FirstGuardianProfession { get; set; }
    public Guid FirstGuardianRelationshipId { get; set; }
    public string FirstGuardianRelationshipName { get; set; }
        
    //father...
    public string SecondGuardianProfession { get; set; }
    public string SecondGuardianEmail { get; set; }
    public string SecondGuardianPhoneNo { get; set; }
    public string SecondGuardianName { get; set; }
    public Guid SecondGuardianRelationshipId { get; set; }
    public string SecondGuardianRelationshipName { get; set; }
        
    // security
    public string FirstGuardianPin { get; set; }
    public string FirstGuardianSecurityQuestion { get; set; }
    public string FirstGuardianSecurityQuestionAnswer { get; set; }
    public string SecondGuardianPin { get; set; }
    public string SecondGuardianSecurityQuestion { get; set; }
    public string SecondGuardianSecurityQuestionAnswer { get; set; }

    public int TenantId { get; set; }
}