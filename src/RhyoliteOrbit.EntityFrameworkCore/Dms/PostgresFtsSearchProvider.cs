using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RhyoliteOrbit.DomainServices.Dms.Search;

namespace RhyoliteOrbit.EntityFrameworkCore.Dms;

/// <summary>
/// PostgreSQL full text search provider. Uses tsvector/ts_rank with plainto_tsquery
/// and metadata filters; snippets are produced with ts_headline.
/// </summary>
public class PostgresFtsSearchProvider: DomainService, ISearchProvider
{
    private const int MinTokenLength = 2;

    private readonly RhyoliteOrbitDbContext _dbContext;
    private readonly IAbpSession _abpSession;

    public PostgresFtsSearchProvider(RhyoliteOrbitDbContext dbContext, IAbpSession abpSession)
    {
        _dbContext = dbContext;
        _abpSession = abpSession;
    }

    public async Task<DmsSearchResponse> SearchAsync(DmsSearchRequest request)
    {
        var startedAt = DateTime.UtcNow;
        var tenantId = _abpSession.TenantId ?? 0;

        var pageNo = request.PageNo < 1 ? 1 : request.PageNo;
        var pageSize = request.PageSize < 1 || request.PageSize > 200 ? 20 : request.PageSize;

        var query = BuildSearchQuery(request.Query);
        var hasQuery = !string.IsNullOrWhiteSpace(query);

        var sql = @"
            SELECT d.""Id""                AS DocumentId,
                   d.""Name""              AS Name,
                   d.""Title""             AS Title,
                   d.""Extension""         AS Extension,
                   d.""FileSize""          AS FileSize,
                   d.""FolderId""          AS FolderId,
                   d.""FolderName""        AS FolderName,
                   d.""CollectionId""      AS CollectionId,
                   d.""CustomId""          AS CustomId,
                   d.""Status""            AS Status,
                   d.""CreationTime""      AS CreationTime,
                   d.""LastModificationTime"" AS LastModificationTime,
                   d.""CreatorUserId""     AS CreatorUserId,
                   " + (hasQuery
                        ? @"COALESCE(ts_rank(d.""SearchVector"", plainto_tsquery('simple', @query)), 0)
                             + COALESCE((SELECT max(ts_rank(v.""SearchVector"", plainto_tsquery('simple', @query)))
                                           FROM ""DmsDocumentVersions"" v
                                          WHERE v.""DocumentId"" = d.""Id""), 0) AS Rank,"
                        : @"0 AS Rank,") + @"
                   " + (hasQuery
                        ? @"COALESCE(ts_headline('simple',
                                 COALESCE(v.""ExtractedText"", d.""Description"", d.""Title"", d.""Name""),
                                 plainto_tsquery('simple', @query),
                                 'StartSel=<b>, StopSel=</b>, MaxWords=40, MinWords=15'), '') AS Snippet,"
                        : @"'' AS Snippet,") + @"
                   (SELECT string_agg(t.""Name"", ',')
                      FROM ""DmsTags"" t
                      JOIN ""DmsDocumentTags"" dt ON dt.""TagId"" = t.""Id""
                     WHERE dt.""DocumentId"" = d.""Id"") AS Tags
              FROM ""Documents"" d
              LEFT JOIN ""DmsDocumentVersions"" v ON v.""Id"" = d.""CurrentVersionId""
             WHERE d.""TenantId"" = @tenantId
               AND d.""IsDeleted"" = @includeDeleted";

        if (request.FolderId.HasValue)
        {
            sql += @" AND d.""FolderId"" = @folderId";
        }

        if (request.CollectionId.HasValue)
        {
            sql += @" AND d.""CollectionId"" = @collectionId";
        }

        if (!string.IsNullOrWhiteSpace(request.Extension))
        {
            sql += @" AND d.""Extension"" = @extension";
        }

        if (!string.IsNullOrWhiteSpace(request.MimeType))
        {
            sql += @" AND d.""MimeType"" = @mimeType";
        }

        if (!string.IsNullOrWhiteSpace(request.CustomId))
        {
            sql += @" AND d.""CustomId"" = @customId";
        }

        if (request.CreatedFrom.HasValue)
        {
            sql += @" AND d.""CreationTime"" >= @createdFrom";
        }

        if (request.CreatedTo.HasValue)
        {
            sql += @" AND d.""CreationTime"" <= @createdTo";
        }

        if (request.CreatedBy.HasValue)
        {
            sql += @" AND d.""CreatorUserId"" = @createdBy";
        }

        if (hasQuery)
        {
            sql += @" AND (d.""SearchVector"" @@ plainto_tsquery('simple', @query)
                        OR v.""SearchVector"" @@ plainto_tsquery('simple', @query))";
        }

        if (request.TagIds != null && request.TagIds.Any())
        {
            sql += @" AND EXISTS (SELECT 1 FROM ""DmsDocumentTags"" dt
                                   WHERE dt.""DocumentId"" = d.""Id""
                                     AND dt.""TagId"" = ANY(@tagIds))";
        }

        if (request.MetadataFilters != null && request.MetadataFilters.Any())
        {
            var index = 0;

            foreach (var filter in request.MetadataFilters)
            {
                sql += $@" AND EXISTS (SELECT 1 FROM ""DmsDocumentMetadataValues"" mv
                                         WHERE mv.""DocumentId"" = d.""Id""
                                           AND mv.""AttributeId"" = @metadataKey{index}
                                           AND mv.""Value"" = @metadataValue{index})";
                index++;
            }
        }

        var orderBy = request.SortOrder switch
        {
            (int)DmsSortOrder.Name => @" ORDER BY d.""Name""",
            (int)DmsSortOrder.Created => @" ORDER BY d.""CreationTime"" DESC",
            (int)DmsSortOrder.Modified => @" ORDER BY d.""LastModificationTime"" DESC NULLS LAST",
            (int)DmsSortOrder.Size => @" ORDER BY d.""FileSize"" DESC",
            _ => hasQuery ? @" ORDER BY Rank DESC, d.""CreationTime"" DESC" : @" ORDER BY d.""CreationTime"" DESC"
        };

        var countSql = $@"SELECT count(*) FROM ""Documents"" d
                           LEFT JOIN ""DmsDocumentVersions"" v ON v.""Id"" = d.""CurrentVersionId""
                          WHERE d.""TenantId"" = @tenantId AND d.""IsDeleted"" = @includeDeleted";

        var hits = new List<DmsSearchHit>();

        await _dbContext.Database.OpenConnectionAsync();

        try
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = BuildCountSql(sql);
                AddParameters(command, request, tenantId, hasQuery, query);

                var total = Convert.ToInt32(await ExecuteScalarAsync(command));

                command.CommandText = sql + orderBy + " LIMIT @pageSize OFFSET @offset";
                AddPagingParameters(command, pageNo, pageSize);

                using var reader = await ExecuteReaderAsync(command);

                while (await reader.ReadAsync())
                {
                    hits.Add(MapHit(reader));
                }

                var facets = await BuildFacetsAsync(tenantId);

                return new DmsSearchResponse
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = total > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0,
                    Data = hits,
                    Facets = facets,
                    ElapsedMs = (DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
        }
        finally
        {
            await _dbContext.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<DmsSuggestion>> SuggestAsync(string prefix, int maxResults)
    {
        if (maxResults <= 0)
        {
            maxResults = 10;
        }

        var term = (prefix ?? string.Empty).Trim().ToLower();

        if (term.Length < MinTokenLength)
        {
            return new List<DmsSuggestion>();
        }

        var names = await _dbContext.Documents
            .Where(d => d.Name.ToLower().StartsWith(term))
            .Select(d => d.Name)
            .Distinct()
            .Take(maxResults)
            .ToListAsync();

        var titles = await _dbContext.Documents
            .Where(d => d.Title != null && d.Title.ToLower().StartsWith(term))
            .Select(d => d.Title)
            .Distinct()
            .Take(maxResults)
            .ToListAsync();

        var tags = await _dbContext.DmsTags
            .Where(t => t.Name.ToLower().StartsWith(term))
            .Select(t => t.Name)
            .Distinct()
            .Take(maxResults)
            .ToListAsync();

        return names.Concat(titles).Concat(tags)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct()
            .Take(maxResults)
            .Select(v => new DmsSuggestion { Text = v, Weight = 1 })
            .ToList();
    }

    public async Task IndexDocumentAsync(Guid documentId)
    {
        // The tsvector columns are maintained by PostgreSQL (GENERATED ALWAYS AS ... STORED),
        // so indexing is a no-op beyond marking the document as indexed.
        var document = await _dbContext.Documents.FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null)
        {
            return;
        }

        document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Indexed;
        document.IndexedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveDocumentAsync(Guid documentId)
    {
        // Documents are soft deleted; removing from the index happens through the
        // IsDeleted filter in the search query.
        await Task.CompletedTask;
    }

    private async Task<Dictionary<string, long>> BuildFacetsAsync(int tenantId)
    {
        var facets = new Dictionary<string, long>();

        var extensions = await _dbContext.Documents
            .Where(d => d.TenantId == tenantId && !d.IsDeleted)
            .GroupBy(d => d.Extension)
            .Select(g => new { Key = g.Key, Count = g.LongCount() })
            .ToListAsync();

        foreach (var extension in extensions.Where(e => !string.IsNullOrEmpty(e.Key)))
        {
            facets[extension.Key] = extension.Count;
        }

        return facets;
    }

    private static string BuildCountSql(string sql)
    {
        // the paging/order clauses are stripped from the data query
        var orderIndex = sql.IndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);

        var body = orderIndex > 0 ? sql.Substring(0, orderIndex) : sql;
        var selectIndex = body.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
        var fromIndex = body.LastIndexOf("FROM", StringComparison.OrdinalIgnoreCase);

        return $"SELECT count(*) {body.Substring(fromIndex)}";
    }

    private static string BuildSearchQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        // strip characters that break the tsquery parser
        var cleaned = new string(query.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray());

        return cleaned.Length < MinTokenLength ? null : cleaned.Trim();
    }

    private static void AddParameters(
        DbCommand command,
        DmsSearchRequest request,
        int tenantId,
        bool hasQuery,
        string query)
    {
        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@includeDeleted", request.IncludeDeleted);

        if (hasQuery)
        {
            AddParameter(command, "@query", query);
        }

        if (request.FolderId.HasValue)
        {
            AddParameter(command, "@folderId", request.FolderId.Value);
        }

        if (request.CollectionId.HasValue)
        {
            AddParameter(command, "@collectionId", request.CollectionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Extension))
        {
            AddParameter(command, "@extension", request.Extension);
        }

        if (!string.IsNullOrWhiteSpace(request.MimeType))
        {
            AddParameter(command, "@mimeType", request.MimeType);
        }

        if (!string.IsNullOrWhiteSpace(request.CustomId))
        {
            AddParameter(command, "@customId", request.CustomId);
        }

        if (request.CreatedFrom.HasValue)
        {
            AddParameter(command, "@createdFrom", request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            AddParameter(command, "@createdTo", request.CreatedTo.Value);
        }

        if (request.CreatedBy.HasValue)
        {
            AddParameter(command, "@createdBy", request.CreatedBy.Value);
        }

        if (request.TagIds != null && request.TagIds.Any())
        {
            AddParameter(command, "@tagIds", request.TagIds.ToArray());
        }

        if (request.MetadataFilters != null && request.MetadataFilters.Any())
        {
            var index = 0;

            foreach (var filter in request.MetadataFilters)
            {
                AddParameter(command, $"@metadataKey{index}", filter.Key);
                AddParameter(command, $"@metadataValue{index}", filter.Value ?? string.Empty);
                index++;
            }
        }
    }

    private static void AddPagingParameters(DbCommand command, int pageNo, int pageSize)
    {
        AddParameter(command, "@pageSize", pageSize);
        AddParameter(command, "@offset", (pageNo - 1) * pageSize);
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }

    private static DmsSearchHit MapHit(DbDataReader reader)
    {
        return new DmsSearchHit
        {
            DocumentId = reader.GetGuid(reader.GetOrdinal("DocumentId")),
            Name = ReadString(reader, "Name"),
            Title = ReadString(reader, "Title"),
            Extension = ReadString(reader, "Extension"),
            FileSize = ReadInt64(reader, "FileSize"),
            FolderId = reader.GetGuid(reader.GetOrdinal("FolderId")),
            FolderName = ReadString(reader, "FolderName"),
            CollectionId = reader.GetGuid(reader.GetOrdinal("CollectionId")),
            CustomId = ReadString(reader, "CustomId"),
            Status = ReadInt32(reader, "Status"),
            CreationTime = reader.GetDateTime(reader.GetOrdinal("CreationTime")),
            LastModificationTime = reader.IsDBNull(reader.GetOrdinal("LastModificationTime"))
                ? (DateTime?)null
                : reader.GetDateTime(reader.GetOrdinal("LastModificationTime")),
            CreatorUserId = reader.IsDBNull(reader.GetOrdinal("CreatorUserId"))
                ? (long?)null
                : reader.GetInt64(reader.GetOrdinal("CreatorUserId")),
            Rank = ReadDouble(reader, "Rank"),
            Snippet = ReadString(reader, "Snippet"),
            Tags = SplitTags(ReadString(reader, "Tags"))
        };
    }

    private static string ReadString(DbDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);

        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static long ReadInt64(DbDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);

        return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt64(reader.GetValue(ordinal));
    }

    private static int ReadInt32(DbDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);

        return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
    }

    private static double ReadDouble(DbDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);

        return reader.IsDBNull(ordinal) ? 0 : Convert.ToDouble(reader.GetValue(ordinal));
    }

    private static List<string> SplitTags(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<string>();
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToList();
    }

    private static Task<object> ExecuteScalarAsync(DbCommand command)
    {
        return command.ExecuteScalarAsync();
    }

    private static Task<DbDataReader> ExecuteReaderAsync(DbCommand command)
    {
        return command.ExecuteReaderAsync();
    }
}
