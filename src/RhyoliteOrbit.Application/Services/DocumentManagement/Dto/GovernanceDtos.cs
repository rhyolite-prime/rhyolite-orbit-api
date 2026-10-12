using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Dto;

// ------------------------------------------------------------------- search
public class DmsSearchInput: PagedAndSortedResultRequestDto
{
    public string Query { get; set; }

    public Guid? FolderId { get; set; }

    public Guid? CollectionId { get; set; }

    public List<Guid> TagIds { get; set; }

    public string Extension { get; set; }

    public string MimeType { get; set; }

    public string CustomId { get; set; }

    public DateTime? CreatedFrom { get; set; }

    public DateTime? CreatedTo { get; set; }

    public long? CreatedBy { get; set; }

    /// <summary>Attribute id -> expected value.</summary>
    public Dictionary<Guid, string> MetadataFilters { get; set; }

    /// <summary>0 = Relevance, 1 = Name, 2 = Created, 3 = Modified, 4 = Size.</summary>
    public int SortOrder { get; set; }

    public bool IncludeDeleted { get; set; }
}

public class DmsSearchResultDto
{
    public int PageNo { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public List<DmsSearchHitDto> Data { get; set; } = new();

    public Dictionary<string, long> Facets { get; set; } = new();

    public double ElapsedMs { get; set; }
}

public class DmsSearchHitDto
{
    public Guid DocumentId { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Extension { get; set; }

    public long FileSize { get; set; }

    public Guid FolderId { get; set; }

    public string FolderName { get; set; }

    public Guid CollectionId { get; set; }

    public string CustomId { get; set; }

    public int Status { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public double Rank { get; set; }

    public string Snippet { get; set; }

    public List<string> Tags { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class DmsSuggestionDto
{
    public string Text { get; set; }

    public int Weight { get; set; }
}

// ------------------------------------------------------- metadata templates
public class DocumentTemplateDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public List<string> AllowedFileTypes { get; set; } = new();

    public Guid? WorkflowTemplateId { get; set; }

    public Guid? RetentionPolicyId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.DocumentVersionScheme"/>.</summary>
    public int VersionScheme { get; set; }

    public bool RequiresApproval { get; set; }
}

public class CreateDocumentTemplateDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> AllowedFileTypes { get; set; } = new();

    public Guid? WorkflowTemplateId { get; set; }

    public Guid? RetentionPolicyId { get; set; }

    public int VersionScheme { get; set; }

    public bool RequiresApproval { get; set; }
}

public class UpdateDocumentTemplateDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> AllowedFileTypes { get; set; } = new();

    public Guid? WorkflowTemplateId { get; set; }

    public Guid? RetentionPolicyId { get; set; }

    public int VersionScheme { get; set; }

    public bool RequiresApproval { get; set; }
}

public class CustomAttributeDto: EntityDto<Guid>
{
    public Guid TemplateId { get; set; }

    public string TemplateName { get; set; }

    public string AttributeName { get; set; }

    public string DataType { get; set; }

    public List<string> PossibleValues { get; set; } = new();

    public string DefaultValue { get; set; }

    public int OrderIndex { get; set; }

    public bool IsRequired { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsSensitive { get; set; }

    public string ValidationRegex { get; set; }
}

public class CreateCustomAttributeDto
{
    public Guid TemplateId { get; set; }

    public string AttributeName { get; set; }

    public string DataType { get; set; }

    public List<string> PossibleValues { get; set; } = new();

    public string DefaultValue { get; set; }

    public int OrderIndex { get; set; }

    public bool IsRequired { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsSensitive { get; set; }

    public string ValidationRegex { get; set; }
}

public class UpdateCustomAttributeDto: EntityDto<Guid>
{
    public string AttributeName { get; set; }

    public string DataType { get; set; }

    public List<string> PossibleValues { get; set; } = new();

    public string DefaultValue { get; set; }

    public int OrderIndex { get; set; }

    public bool IsRequired { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsSensitive { get; set; }

    public string ValidationRegex { get; set; }
}

public class MetadataValueDto
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; }

    public string Value { get; set; }
}

// ------------------------------------------------------------- identifiers
public class CustomIdentifierDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool IncludeYear { get; set; }

    public bool IncludeMonth { get; set; }

    public int PaddingLength { get; set; }

    public int CurrentSequence { get; set; }

    public Guid TemplateId { get; set; }

    public string TemplateName { get; set; }

    public bool IsActive { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.SequenceResetPolicy"/>.</summary>
    public int ResetPolicy { get; set; }

    public DateTime? LastGeneratedAt { get; set; }
}

public class CreateCustomIdentifierDto
{
    public string Name { get; set; }

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool IncludeYear { get; set; }

    public bool IncludeMonth { get; set; }

    public int PaddingLength { get; set; } = 5;

    public Guid TemplateId { get; set; }

    public bool IsActive { get; set; } = true;

    public int ResetPolicy { get; set; }
}

public class UpdateCustomIdentifierDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool IncludeYear { get; set; }

    public bool IncludeMonth { get; set; }

    public int PaddingLength { get; set; } = 5;

    public Guid TemplateId { get; set; }

    public bool IsActive { get; set; }

    public int ResetPolicy { get; set; }
}

// --------------------------------------------------------------- permissions
public class AclEntryDto: EntityDto<Guid>
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.SecurableType"/>.</summary>
    public int SecurableType { get; set; }

    public Guid SecurableId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AclSubjectType"/>.</summary>
    public int SubjectType { get; set; }

    public long SubjectId { get; set; }

    public int Permission { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AclEffect"/>.</summary>
    public int Effect { get; set; }

    public bool IsInherited { get; set; }
}

public class GrantPermissionDto
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.SecurableType"/>.</summary>
    public int SecurableType { get; set; }

    public Guid SecurableId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AclSubjectType"/>.</summary>
    public int SubjectType { get; set; }

    public long SubjectId { get; set; }

    public int Permission { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AclEffect"/>.</summary>
    public int Effect { get; set; }
}

public class EffectivePermissionsDto
{
    public int Permission { get; set; }
}

// ---------------------------------------------------------------- retention
public class RetentionPolicyDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.RetentionTrigger"/>.</summary>
    public int Trigger { get; set; }

    public int RetentionDays { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.DispositionAction"/>.</summary>
    public int DispositionAction { get; set; }

    public bool AllowLegalHold { get; set; }

    public bool IsActive { get; set; }

    public List<Guid> FolderIds { get; set; } = new();
}

public class CreateRetentionPolicyDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int Trigger { get; set; }

    public int RetentionDays { get; set; }

    public int DispositionAction { get; set; }

    public bool AllowLegalHold { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public List<Guid> FolderIds { get; set; } = new();
}

public class UpdateRetentionPolicyDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int Trigger { get; set; }

    public int RetentionDays { get; set; }

    public int DispositionAction { get; set; }

    public bool AllowLegalHold { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public List<Guid> FolderIds { get; set; } = new();
}

public class LegalHoldDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Reason { get; set; }

    public List<Guid> DocumentIds { get; set; } = new();

    public List<Guid> FolderIds { get; set; } = new();

    public long AppliedById { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreateLegalHoldDto
{
    public string Name { get; set; }

    public string Reason { get; set; }

    public List<Guid> DocumentIds { get; set; } = new();

    public List<Guid> FolderIds { get; set; } = new();
}

// --------------------------------------------------------------- signatures
public class SignatureDto: EntityDto<Guid>
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public long SignerId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.SignatureType"/>.</summary>
    public int SignatureType { get; set; }

    public string CertificateThumbprint { get; set; }

    public string SignedHash { get; set; }

    public DateTime SignedAt { get; set; }

    public string IpAddress { get; set; }

    public string Reason { get; set; }
}

public class SignDocumentDto
{
    public Guid DocumentId { get; set; }

    public Guid? VersionId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.SignatureType"/>.</summary>
    public int SignatureType { get; set; }

    /// <summary>Base64 encoded PNG for drawn/typed/uploaded signatures.</summary>
    public string SignatureImageBase64 { get; set; }

    public string CertificateThumbprint { get; set; }

    public string Reason { get; set; }
}

// ------------------------------------------------------------- entity links
public class EntityLinkDto: EntityDto<Guid>
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.DmsEntityType"/>.</summary>
    public int EntityType { get; set; }

    public string EntityId { get; set; }

    public Guid DocumentId { get; set; }

    public string DocumentName { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.DmsLinkType"/>.</summary>
    public int LinkType { get; set; }

    public string Note { get; set; }

    public long CreatedById { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreateEntityLinkDto
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.DmsEntityType"/>.</summary>
    public int EntityType { get; set; }

    public string EntityId { get; set; }

    public Guid DocumentId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.DmsLinkType"/>.</summary>
    public int LinkType { get; set; }

    public string Note { get; set; }
}

// ------------------------------------------------------------------ audit
public class AuditEntryDto: EntityDto<Guid>
{
    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AuditAction"/>.</summary>
    public int Action { get; set; }

    public long? ActorId { get; set; }

    public string DetailsJson { get; set; }

    public string IpAddress { get; set; }

    public long Sequence { get; set; }

    public string EntryHash { get; set; }

    public DateTime CreationTime { get; set; }
}

public class AuditChainVerificationDto
{
    public bool IsValid { get; set; }

    public int VerifiedEntries { get; set; }

    public string FirstBrokenSequence { get; set; }
}

// ------------------------------------------------------------ settings/quota
public class DmsSettingsDto: EntityDto<Guid>
{
    public string StorageProvider { get; set; }

    public string LocalRootPath { get; set; }

    public List<string> AllowedFileTypes { get; set; } = new();

    public int MaxFileSizeMb { get; set; }

    public int QuotaGb { get; set; }

    public bool OcrEnabled { get; set; }

    public List<string> OcrLanguages { get; set; } = new();

    public bool AntivirusEnabled { get; set; }

    public bool AiEnabled { get; set; }

    public string WatermarkText { get; set; }

    public int TrashRetentionDays { get; set; }

    public bool EnforceVersioning { get; set; }

    public string DefaultLanguage { get; set; }
}

public class UpdateDmsSettingsDto
{
    public string StorageProvider { get; set; }

    public string LocalRootPath { get; set; }

    public List<string> AllowedFileTypes { get; set; } = new();

    public int MaxFileSizeMb { get; set; }

    public int QuotaGb { get; set; }

    public bool OcrEnabled { get; set; }

    public List<string> OcrLanguages { get; set; } = new();

    public bool AntivirusEnabled { get; set; }

    public bool AiEnabled { get; set; }

    public string WatermarkText { get; set; }

    public int TrashRetentionDays { get; set; }

    public bool EnforceVersioning { get; set; }

    public string DefaultLanguage { get; set; }
}

public class QuotaUsageDto
{
    public long BytesUsed { get; set; }

    public long QuotaBytes { get; set; }

    public int DocumentCount { get; set; }

    public int VersionCount { get; set; }

    public DateTime ComputedAt { get; set; }

    public double UsedPercentage { get; set; }
}

// ---------------------------------------------------------------- bulk i/o
public class IngestJobDto: EntityDto<Guid>
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.IngestJobType"/>.</summary>
    public int JobType { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.IngestJobStatus"/>.</summary>
    public int Status { get; set; }

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public int FailedItems { get; set; }

    public string SourcePath { get; set; }

    public string ResultPath { get; set; }

    public string ErrorLog { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreationTime { get; set; }
}

public class ImportZipDto
{
    public Guid FolderId { get; set; }

    public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
}

// ------------------------------------------------------------------- AI
public class DocumentClassificationDto
{
    public string SuggestedName { get; set; }

    public string SuggestedDocumentType { get; set; }

    public string Summary { get; set; }

    public string DetectedLanguage { get; set; }

    public List<string> SuggestedTags { get; set; } = new();
}

public class AiQuestionDto
{
    public string Question { get; set; }

    public int MaxChunks { get; set; } = 5;
}

public class AiAnswerDto
{
    public string Answer { get; set; }

    public List<AiCitationDto> Citations { get; set; } = new();
}

public class AiCitationDto
{
    public Guid DocumentId { get; set; }

    public string DocumentName { get; set; }

    public Guid VersionId { get; set; }

    public double Score { get; set; }

    public string Excerpt { get; set; }
}

// -------------------------------------------------------------- dashboard
public class DmsDashboardDto
{
    public long TotalDocuments { get; set; }

    public long TotalFolders { get; set; }

    public long TotalCollections { get; set; }

    public long BytesUsed { get; set; }

    public long QuotaBytes { get; set; }

    public long PendingWorkflowTasks { get; set; }

    public long DocumentsOnLegalHold { get; set; }

    public long DocumentsExpiringSoon { get; set; }

    public long UnindexedDocuments { get; set; }
}
