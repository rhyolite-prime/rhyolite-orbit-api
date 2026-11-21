using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A chunk of document text together with its vector embedding, used by semantic
/// search and retrieval augmented generation.
/// </summary>
public class DmsEmbedding: Abp.Domain.Entities.Auditing.CreationAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public int ChunkIndex { get; set; }

    public string ChunkText { get; set; }

    /// <summary>JSON serialized float array. Swap for pgvector when available.</summary>
    [Column(TypeName = "jsonb")]
    public string VectorJson { get; set; }

    [MaxLength(50)]
    public string Model { get; set; }

    public int TenantId { get; set; }
}
