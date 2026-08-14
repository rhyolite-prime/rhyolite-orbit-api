using System;

namespace RhyoliteOrbit.Models.Wssd;

public class DefaultDatasetModel
{
    public int Id { get; set; }
    
    public string Key { get; set; }
    
    public string Value { get; set; }
    
    public string DataType { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
}