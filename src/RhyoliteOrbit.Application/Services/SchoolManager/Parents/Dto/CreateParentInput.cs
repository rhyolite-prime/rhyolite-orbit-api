using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Parents.Dto;

public class CreateParentInput
{
    public string FirstGuardianName { get; set; }
    public string FirstGuardianPhoneNo { get; set; }
    public string FirstGuardianEmail { get; set; }
    
    public string FirstGuardianProfession { get; set; }
    public string FirstGuardianRelationship { get; set; }
        
    //father...
    public string SecondGuardianProfession { get; set; }
    public string SecondGuardianEmail { get; set; }
    public string SecondGuardianPhoneNo { get; set; }
    public string SecondGuardianName { get; set; }
    public string SecondGuardianRelationship { get; set; }
        
    // security
    public string FirstGuardianPin { get; set; }
    
    public string FirstGuardianSecurityQuestion { get; set; }
    
    public string FirstGuardianSecurityQuestionAnswer { get; set; }
    
    public string SecondGuardianPin { get; set; }
    
    public string SecondGuardianSecurityQuestion { get; set; }
    
    public string SecondGuardianSecurityQuestionAnswer { get; set; }
}

public class UpdateParentInput: EntityDto<Guid>
{
    public string FirstGuardianName { get; set; }
    public string FirstGuardianPhoneNo { get; set; }
    public string FirstGuardianEmail { get; set; }
    
    public string FirstGuardianProfession { get; set; }
    public string FirstGuardianRelationship { get; set; }
        
    //father...
    public string SecondGuardianProfession { get; set; }
    public string SecondGuardianEmail { get; set; }
    public string SecondGuardianPhoneNo { get; set; }
    public string SecondGuardianName { get; set; }
    public string SecondGuardianRelationship { get; set; }
        
    // security
    public string FirstGuardianPin { get; set; }
    
    public string FirstGuardianSecurityQuestion { get; set; }
    
    public string FirstGuardianSecurityQuestionAnswer { get; set; }
    
    public string SecondGuardianPin { get; set; }
    
    public string SecondGuardianSecurityQuestion { get; set; }
    
    public string SecondGuardianSecurityQuestionAnswer { get; set; }
}