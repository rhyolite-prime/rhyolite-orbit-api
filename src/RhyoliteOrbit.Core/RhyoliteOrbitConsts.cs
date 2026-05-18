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
        Active = 0,
        Inactive = 1,
        UnderMaintenance = 2,
        UnderConstruction = 3,
        Closed = 4
    }
    
    //SportsTeam status
    public enum TeamStatus
    {
        Active = 0,
        Inactive = 1,
        Suspended = 2,
        Disbanded = 3,
        OnBreak = 4
    }

    //ProductionOrder status
    public enum ProductionOrderStatus
    {
        Planned = 0,
        InProgress = 1,
        Completed = 2
    }
    
    
    //QualityControl status
    public enum QualityControlStatus
    {
        Pending = 0,
        Pass = 1,
        Fail = 2
    }
    
    public enum QualityControlTestType
    {
        Visual = 0,
        Dimensional = 1,
        Material = 2,
        Texture = 3,
        Color = 4,
        Other = 5,
    }
    
    public enum QualityControlTestResult
    {
        Passed = 0,
        Failed = 1,
        Inconclusive = 2,
        Cancelled = 3
    }

    
    // student status
    public enum StudentStatus
    {
        Active = 0,
        Inactive = 1,
        Graduated = 2,
        Suspended = 3,
        Expelled = 4,
    }
    
    //employee status
    public enum EmployeeStatus
    {
        Active = 0,
        Inactive = 1,
        OnLeave = 2,
        Resigned = 3,
        Retired = 4,
        Hospitalized = 5
    }
    
    public enum ErpModule
    {
       SchoolInformationSystem = 1,
       PayrollSystem = 2,
       GeneralLedger = 3,
       AccountsPayable = 4,
       AccountsReceivable = 5,
       FixedAssets = 6,
       PointOfSale = 7,
       HumanResource = 8,
       Inventory = 9,
       SupplyChain = 10,
       Manufacturing = 11,
       PropertyOrAssetRental = 12,
    }
    
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5,
        PartiallyRefunded = 6,
        OnHold = 7,
        Overdue = 8
    }
    
    public enum WorkflowStepType
    {
        Submission = 0,      // Initial document upload/creation
        Approval = 1,        // Standard approval
        Review = 2,          // Review without approval authority
        Signature = 3,       // Digital signature required
        Notification = 4,    // Informational only, no action required
        Condition = 5,       // Automatic decision point
        Parallel = 6,        // Multiple simultaneous approvals
        Sequential = 7,      // Ordered approvals
        Escalation = 8       // Timeout handling step
    }

    public enum CompletionRule
    {
        Any = 0,             // First to respond wins
        All = 1,             // Everyone must approve
        Majority = 2,        // >50% approval
        Percentage = 3,      // Custom percentage (e.g., 75%)
        SpecificUsers = 4    // Specific users must approve
    }

    public enum StepActionType
    {
        Approve = 0,
        Reject = 1,
        RequestChanges = 2,
        Assign = 3,
        Delegate = 4,
        AddComment = 5,
        UploadAttachment = 6
    }
    
    public enum ConditionField
    {
        DocumentMetadata,
        DocumentProperty,
        UserProperty,
        SystemVariable,
        PreviousStepOutcome
    }
    
    public enum ConditionOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        Contains,
        StartsWith,
        EndsWith,
        In,
        NotIn
    }
    
    public enum WorkflowTaskType
    {
        Approval = 0,

        Review = 1,

        Signature = 2,

        Submission = 3,

        Notification = 4,

        Escalation = 5
    }
    
    public enum WorkflowTaskStatus
    {
        Pending = 0,

        Completed = 1,

        Rejected = 2,

        RequestChanges = 3,

        Skipped = 4,

        Expired = 5
    }
    public enum AssigneeType
    {
        SpecificUser = 0,      // Fixed user ID
        Role = 1,              // All users in role
        Department = 2,        // Department head or members
        DocumentCreator = 3,   // User who created the document
        ManagerOfCreator = 4,  // Direct manager
        Dynamic = 5            // Determined by document metadata
    }
    
    public enum SubscriptionRenewalStatus
    {
        Pending = 0,     // renewal scheduled/queued but not attempted yet
        Processing = 1,  // currently attempting payment/renewal
        Renewed = 2,     // renewal succeeded
        Failed = 3,      // renewal attempt failed
        Cancelled = 4,   // cancelled by user/system before completion
        Skipped = 5,     // intentionally not renewed (e.g., opted out, not due)
        Expired = 6,     // subscription expired without successful renewal
        Refunded = 7     // renewal was reversed after success
    }
    
    public enum BillOfMaterialType
    {
        Raw = 0,
        SemiFinished = 1,
        Consumable = 2
    }

    public enum Priority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
    
    public enum OperationStatus 
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Overdue = 4,
        OnHold = 5,
        Revised = 6
    }

    public enum ScrapType
    {
        /// <summary>
        /// Material lost due to defects in the process (e.g. broken, damaged, misaligned).
        /// </summary>
        Defect = 0,

        /// <summary>
        /// Extra material cut off during processing (e.g. offcuts, trimmings).
        /// </summary>
        Offcut = 1,

        /// <summary>
        /// Normal/inevitable waste from the process (e.g. dust, shavings, residues).
        /// </summary>
        ProcessWaste = 2,

        /// <summary>
        /// Items rejected during quality control inspections.
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Other unclassified scrap or waste.
        /// </summary>
        Other = 4
    }
    
    public enum SeverityLevel
    {
        Minor = 1,      // Small issue, low impact, no interruption
        Major = 2,      // Significant issue, may cause rework or delays
        Critical = 3    // Severe issue, halts production or impacts safety/compliance
    }
    
    public enum InProcessInspectionStatus
    {
        Pending = 0,      // Inspection not yet performed
        InProgress = 1,   // Currently being inspected
        Passed = 2,       // Inspection passed successfully
        Failed = 3,       // Inspection failed, requires action
        ReworkRequired = 4, // Failed but can be reworked
        Cancelled = 5     // Inspection was cancelled/voided
    }
    
    public enum NonConformanceStatus
    {
        Open = 0,         // Non-conformance identified, awaiting review
        UnderReview = 1,  // Being investigated for root cause
        ActionInProgress = 2, // Corrective/Preventive actions are being implemented
        Resolved = 3,     // Actions completed, awaiting verification
        Closed = 4,       // Verified and officially closed
        Cancelled = 5     // Logged in error or invalid
    }
    
    public enum FinalProductTestStatus
    {
        Pending = 0,     // Test not yet performed
        InProgress = 1,  // Test is currently being executed
        Approved = 2,    // Product passed all required tests
        Rejected = 3,    // Product failed tests and cannot be released
        OnHold = 4,      // Test completed, but product release pending decision
        Cancelled = 5    // Test was voided or not applicable
    }
    
    public enum InventoryCategory
    {
        RawMaterial = 0,          // Basic unprocessed inputs
        SemiFinishedGood = 1,     // Work-in-progress or partly processed items
        FinishedGood = 2,         // Ready for sale/distribution
        PackagingMaterial = 3,    // Boxes, labels, containers, cartons
        Consumable = 4,           // Items used up in production (glue, paint, solvents)
        SparePart = 5,            // Machine spares, replacement parts
        ToolAndEquipment = 6,     // Tools used for manufacturing but not sold
        Energy = 7                 // Electricity, fuel, gas, renewable sources tracked as inventory
    }

    
    


     
    
    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "f9be3a4eed7441d1bb1da71898a30d95";

    public static string GetStudentStatusString(StudentStatus status)
    {
        return status.ToString();
    }
}
