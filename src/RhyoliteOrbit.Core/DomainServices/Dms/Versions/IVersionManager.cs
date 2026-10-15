using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Versions;

/// <summary>Input used when new content is attached to a document.</summary>
public class VersionUploadInput
{
    public Stream Content { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public string ChangeNote { get; set; }

    /// <summary>0 = Major, 1 = Minor (see RhyoliteOrbitConsts.DocumentVersionScheme).</summary>
    public int Scheme { get; set; }

    public long CreatedById { get; set; }
}

public interface IVersionManager
{
    Task<DmsDocumentVersion> CreateInitialAsync(DmsDocument document, VersionUploadInput input);

    Task<DmsDocumentVersion> AddAsync(Guid documentId, VersionUploadInput input);

    Task PromoteAsync(Guid versionId, long promotedBy);

    Task<List<DmsDocumentVersion>> ListAsync(Guid documentId);

    Task<DmsDocumentVersion> GetAsync(Guid versionId);

    Task<DmsDocumentVersion> GetCurrentAsync(Guid documentId);

    Task<Stream> OpenReadAsync(Guid versionId);

    Task UpdateIndexStateAsync(Guid versionId, string extractedText, bool ocrApplied, int pageCount, string language);

    Task DeleteAsync(Guid versionId, long deletedBy);
}
