using System.Collections.Generic;
using RhyoliteOrbit.Debugging;

namespace RhyoliteOrbit;

public class RhyoliteOrbitConsts
{
    public const string LocalizationSourceName = "RhyoliteOrbit";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;
    
    public enum BillStatus
    {
        Draft = 0,
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4,
        Paid = 5,
        PartiallyPaid = 6,
        Refunded = 7,
        PartiallyRefunded = 8
    }
    
    public enum AccountHeadTypeDefinitions
    {
        Assets = 0,
        Liabilities = 1,
        Equity = 2,
        Revenue = 3,
        Expenses = 4,
    }
    
    public enum BusinessPartnerType
    {
        Customer = 1,
        Supplier = 2,
        Both = 3
    }
    
    //facility status
    public enum FacilityStatus
    {
        Available = 0,
        Occupied = 1,
        UnderMaintenance = 2,
        OutOfService = 3
    }
    
    //LaboratoryEquipment condition
    public enum EquipmentCondition
    {
        New = 0,
        Good = 1,
        Fair = 2,
        Poor = 3,
        Broken = 4
    }
    
    public enum LaboratoryStatus
    {
        Active = 1,
        Inactive = 2,
        UnderMaintenance = 3,
        UnderConstruction = 4,
        Closed = 5
    }
    
    //SportsTeam status
    public enum TeamStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Disbanded = 4,
        OnBreak = 5
    }


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "f9be3a4eed7441d1bb1da71898a30d95";
}
