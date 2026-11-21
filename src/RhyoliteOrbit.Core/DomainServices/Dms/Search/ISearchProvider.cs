using System.Collections.Generic;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Search;

/// <summary>
/// Full text search over document content and metadata. The default implementation
/// uses PostgreSQL full text search; an OpenSearch/Elasticsearch provider can be
/// plugged in without changing callers.
/// </summary>
public interface ISearchProvider
{
    Task<DmsSearchResponse> SearchAsync(DmsSearchRequest request);

    Task<List<DmsSuggestion>> SuggestAsync(string prefix, int maxResults);

    /// <summary>Queues (or performs) indexing of a document.</summary>
    Task IndexDocumentAsync(System.Guid documentId);

    Task RemoveDocumentAsync(System.Guid documentId);
}
