namespace RhyoliteOrbit.Services.Pos.Customers.Dto;

public class PosCustomerDto
{
    public string Username { get; set; }    
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
        
    public string PhoneNumber { get; set; }
    
    public string Email { get; set; }
    
    public string Pin { get; set; }
    
}

public class PosCustomerLoginDto
{
    public string AccountIdentifier { get; set; }
    
    public string Password { get; set; }
}