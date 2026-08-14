using System;

namespace RhyoliteOrbit.Models.Wssd;

public class InteractionDetail
{
    public Guid MenuId { get; set; }
    
    public string MenuName { get; set; }
    
    public Guid SelectedOptionId { get; set; }
   
    public string SelectedOptionTitle { get; set; }
   
    public string ActionType { get; set; }
     
    public string UserInput { get; set; }
     
    public string SystemResponse { get; set; }
    
    public bool IsSuccessful { get; set; }
    
    public DateTime InteractionTime { get; set; }
}