using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Guardian
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public string Profession { get; set; }
    public string Relationship { get; set; }
        
    public string Pin { get; set; }
    
    public string SecurityQuestion { get; set; }
    
    public string SecurityQuestionAnswer { get; set; }
}