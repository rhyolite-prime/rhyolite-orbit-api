using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;

namespace RhyoliteOrbit.Services.DocumentManagement;

/// <summary>
/// Entity -> DTO mappings for the document management module. The assembly is
/// scanned by ABP's AutoMapper integration (see RhyoliteOrbitApplicationModule).
/// </summary>
public class DmsMapProfile: Profile
{
    public DmsMapProfile()
    {
        // ---------------------------------------------------------- documents
        CreateMap<Models.Dms.DmsDocument, Dto.DocumentDto>()
            .ForMember(d => d.Tags, o => o.Ignore())
            .ForMember(d => d.CurrentVersionLabel, o => o.Ignore());

        CreateMap<Models.Dms.DmsDocumentVersion, Dto.DocumentVersionDto>();

        // ----------------------------------------------------------- folders
        CreateMap<Models.Dms.DmsFolder, Dto.FolderDto>()
            .ForMember(d => d.Children, o => o.Ignore());

        CreateMap<Models.Dms.DmsCollection, Dto.CollectionDto>();
        CreateMap<Models.Dms.DmsTag, Dto.TagDto>();

        // ------------------------------------------------------ collaboration
        CreateMap<Models.Dms.DmsShare, Dto.ShareDto>();
        CreateMap<Models.Dms.DmsComment, Dto.CommentDto>();
        CreateMap<Models.Dms.DmsAnnotation, Dto.AnnotationDto>();
        CreateMap<Models.Dms.DmsSavedSearch, Dto.SavedSearchDto>();

        // ----------------------------------------------------------- metadata
        CreateMap<Models.Dms.DmsDocumentTemplate, Dto.DocumentTemplateDto>();
        CreateMap<Models.Dms.DmsCustomAttributes, Dto.CustomAttributeDto>();
        CreateMap<Models.Dms.DmsCustomIdentifier, Dto.CustomIdentifierDto>();
        CreateMap<Models.Dms.DmsAclEntry, Dto.AclEntryDto>();

        // ---------------------------------------------------------- workflow
        CreateMap<Models.Dms.DmsWorkflowTemplate, Dto.WorkflowTemplateDto>()
            .ForMember(d => d.Steps, o => o.MapFrom(s => s.WorkflowTemplateSteps));

        CreateMap<Models.Dms.WorkflowTemplateStep, Dto.WorkflowTemplateStepDto>();
        CreateMap<Models.Dms.StepAssignee, Dto.StepAssigneeDto>();
        CreateMap<Models.Dms.StepCondition, Dto.StepConditionDto>();
        CreateMap<Models.Dms.StepAction, Dto.StepActionDto>();

        CreateMap<Models.Dms.DmsWorkflowInstance, Dto.WorkflowInstanceDto>();
        CreateMap<Models.Dms.DmsWorkflowTask, Dto.WorkflowTaskDto>();

        // --------------------------------------------------------- governance
        CreateMap<Models.Dms.DmsRetentionPolicy, Dto.RetentionPolicyDto>();
        CreateMap<Models.Dms.DmsLegalHold, Dto.LegalHoldDto>();
        CreateMap<Models.Dms.DmsSignature, Dto.SignatureDto>();
        CreateMap<Models.Dms.DmsEntityLink, Dto.EntityLinkDto>();
        CreateMap<Models.Dms.DmsAuditEntry, Dto.AuditEntryDto>();
        CreateMap<Models.Dms.DmsSettings, Dto.DmsSettingsDto>();
        CreateMap<Models.Dms.DmsQuotaUsage, Dto.QuotaUsageDto>()
            .ForMember(d => d.QuotaBytes, o => o.Ignore())
            .ForMember(d => d.UsedPercentage, o => o.Ignore());
        CreateMap<Models.Dms.DmsIngestJob, Dto.IngestJobDto>();

        // ------------------------------------------------------------ search
        CreateMap<DomainServices.Dms.Search.DmsSearchHit, Dto.DmsSearchHitDto>();
        CreateMap<DomainServices.Dms.Search.DmsSuggestion, Dto.DmsSuggestionDto>();

        // ---------------------------------------------------------------- ai
        CreateMap<DomainServices.Dms.Ai.DocumentClassification, Dto.DocumentClassificationDto>();
    }
}
