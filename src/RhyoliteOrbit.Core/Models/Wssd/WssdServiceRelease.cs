using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdServiceRelease: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid WssdServiceId { get; set; }
    
    [MaxLength(100)]
    public string WssdServiceName { get; set; }
    
    //if staging, development is used, user will be prompted wo add email/phone number or any identity to set it aside from the outside world 
    //if production, wssd config will be synced to the live redis server
    //for development, staging, the wssd config will remain in the database
    // a random url will be generated for the service that can be accessed and used for testing purposes
    [MaxLength(25)]
    public string Environment { get; set; } //staging, production, development
    
    [Column(TypeName = "jsonb")] public List<string> WhitelistedPhoneNumbers { get; set; } // for ussd/wssd 
    
    [Column(TypeName = "jsonb")] public List<string> WhitelistedEmails { get; set; } // for wssd only
     
    [MaxLength(130)]
    public string CreatedBy { get; set; }
    
    [MaxLength(130)]
    public string ApprovedBy { get; set; }
    
    public DateTime TimeApproved  { get; set; }
    
    [MaxLength(250)]
    public string ReleaseNotes { get; set; }
    
    public int Status { get; set; } 
    
    [MaxLength(100)]
    public string ArtefactVersion { get; set; } // can be controlled from system numbers: must be unique for each release, must be incremental

    public Guid? RollBackTo { get; set; } // id of a specific release to roll back to, if applicable
    
    public DateTime? RollBackDate { get; set; }
    
    public decimal ErrorRateThreshold { get; set; } // e.g., 0.05 for 5%
    
    public decimal CurrentErrorRate { get; set; } // updated by monitoring logic
    
    public bool IsActive { get; set; }
    
    public int TenantId { get; set; }
}