using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.DomainServices.Dms.Search;

/// <summary>Full text + metadata search request.</summary>
public class DmsSearchRequest
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

    /// <summary>Attribute filters: attribute id -> expected value.</summary>
    public Dictionary<Guid, string> MetadataFilters { get; set; }

    /// <summary>0 = Relevance, 1 = Name, 2 = Created, 3 = Modified, 4 = Size (see DmsSortOrder).</summary>
    public int SortOrder { get; set; }

    public bool IncludeDeleted { get; set; }

    public int PageNo { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public enum DmsSortOrder
{
    Relevance = 0,
    Name = 1,
    Created = 2,
    Modified = 3,
    Size = 4
}

/// <summary>A single search hit.</summary>
public class DmsSearchHit
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

    /// <summary>Highlighted text snippet (HTML &lt;b&gt; marks).</summary>
    public string Snippet { get; set; }

    public List<string> Tags { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}

public class DmsSearchResponse
{
    public int PageNo { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public List<DmsSearchHit> Data { get; set; }

    public Dictionary<string, long> Facets { get; set; }

    public double ElapsedMs { get; set; }
}

/// <summary>Autocomplete suggestion.</summary>
public class DmsSuggestion
{
    public string Text { get; set; }

    public int Weight { get; set; }
}
