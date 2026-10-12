using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Transactional outbox entry. Written inside the same unit of work as the domain change
/// and published to RabbitMQ by a dispatcher.
/// </summary>
public class DmsDocumentEvent: Abp.Domain.Entities.Auditing.CreationAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string EventType { get; set; }

    public System.Guid AggregateId { get; set; }

    [Column(TypeName = "jsonb")]
    public string PayloadJson { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int RetryCount { get; set; }

    [MaxLength(500)]
    public string LastError { get; set; }

    public int TenantId { get; set; }
}
