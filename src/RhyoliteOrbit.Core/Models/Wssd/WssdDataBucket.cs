using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdDataBucket: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(30)]
    public string DataSetType { get; set; } // xls, xlsx, csv, json, xml
    [MaxLength(500)]
    public string Description { get; set; }    
    
    public Guid ParentBucketId { get; set; }
    
    [Column(TypeName = "jsonb")] public List<DefaultDatasetModel> Dataset { get; set; } // maximum limit per dataset is 250,000 records
    
    public int TenantId { get; set; }
}