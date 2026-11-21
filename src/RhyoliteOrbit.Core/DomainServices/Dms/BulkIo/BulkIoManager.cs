using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Folders;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.BulkIo;

public interface IBulkIoManager
{
    Task<DmsIngestJob> ImportZipAsync(Guid folderId, Stream zipStream, string fileName, long createdBy);

    Task<DmsIngestJob> ExportZipAsync(Guid folderId, long createdBy);

    Task<DmsIngestJob> GetJobAsync(Guid jobId);

    Task<List<DmsIngestJob>> ListJobsAsync(int pageNo, int pageSize);
}

/// <summary>
/// Bulk import and export of folder trees as ZIP archives. Import creates the
/// folder structure contained in the archive and uploads each file as a document.
/// </summary>
public class BulkIoManager: DomainService, IBulkIoManager
{
    private readonly IRepository<DmsIngestJob, Guid> _jobRepository;
    private readonly IDocumentManager _documentManager;
    private readonly IFolderManager _folderManager;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly IAbpSession _abpSession;

    public BulkIoManager(
        IRepository<DmsIngestJob, Guid> jobRepository,
        IDocumentManager documentManager,
        IFolderManager folderManager,
        IDmsSettingsManager settingsManager,
        IAbpSession abpSession)
    {
        _jobRepository = jobRepository;
        _documentManager = documentManager;
        _folderManager = folderManager;
        _settingsManager = settingsManager;
        _abpSession = abpSession;
    }

    public async Task<DmsIngestJob> ImportZipAsync(Guid folderId, Stream zipStream, string fileName, long createdBy)
    {
        var targetFolder = await _folderManager.GetAsync(folderId);
        if (targetFolder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
        }

        var job = new DmsIngestJob
        {
            TenantId = _abpSession.TenantId ?? 0,
            JobType = (int)RhyoliteOrbitConsts.IngestJobType.ImportZip,
            Status = (int)RhyoliteOrbitConsts.IngestJobStatus.Running,
            SourcePath = fileName,
            StartedAt = DateTime.UtcNow,
            CreatedById = createdBy
        };

        job = await _jobRepository.InsertAsync(job);

        var folderCache = new Dictionary<string, Guid> { [string.Empty] = targetFolder.Id };
        var errors = new List<string>();

        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, true);

        job.TotalItems = archive.Entries.Count;

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                // directory entry
                continue;
            }

            try
            {
                var directory = GetDirectoryName(entry.FullName);

                if (!folderCache.TryGetValue(directory, out var parentId))
                {
                    parentId = await EnsureFolderPathAsync(targetFolder.Id, directory);
                    folderCache[directory] = parentId;
                }

                await using var entryStream = entry.Open();
                await using var buffer = new MemoryStream();
                await entryStream.CopyToAsync(buffer);
                buffer.Position = 0;

                await _documentManager.UploadAsync(new DocumentUploadInput
                {
                    FolderId = parentId,
                    Name = entry.Name,
                    FileName = entry.Name,
                    Content = buffer,
                    ContentType = "application/octet-stream",
                    CreatedById = createdBy
                });

                job.ProcessedItems++;
            }
            catch (Exception ex)
            {
                job.FailedItems++;
                errors.Add($"{entry.FullName}: {ex.Message}");
            }
        }

        job.Status = job.FailedItems == 0
            ? (int)RhyoliteOrbitConsts.IngestJobStatus.Completed
            : (int)RhyoliteOrbitConsts.IngestJobStatus.Failed;

        job.CompletedAt = DateTime.UtcNow;
        job.ErrorLog = errors.Any() ? string.Join("\n", errors.Take(100)) : null;

        await _jobRepository.UpdateAsync(job);

        return job;
    }

    public async Task<DmsIngestJob> ExportZipAsync(Guid folderId, long createdBy)
    {
        var folder = await _folderManager.GetAsync(folderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
        }

        var job = new DmsIngestJob
        {
            TenantId = _abpSession.TenantId ?? 0,
            JobType = (int)RhyoliteOrbitConsts.IngestJobType.ExportZip,
            Status = (int)RhyoliteOrbitConsts.IngestJobStatus.Running,
            SourcePath = folder.Path,
            StartedAt = DateTime.UtcNow,
            CreatedById = createdBy
        };

        job = await _jobRepository.InsertAsync(job);

        try
        {
            var exportDirectory = Path.Combine(Path.GetTempPath(), "dms-exports");
            Directory.CreateDirectory(exportDirectory);

            var zipPath = Path.Combine(exportDirectory, $"{folder.Name}_{Guid.NewGuid():N}.zip");

            using (var zipStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
            {
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);

                await AddFolderToArchiveAsync(archive, folder, string.Empty);
            }

            job.ResultPath = zipPath;
            job.Status = (int)RhyoliteOrbitConsts.IngestJobStatus.Completed;
        }
        catch (Exception ex)
        {
            job.Status = (int)RhyoliteOrbitConsts.IngestJobStatus.Failed;
            job.ErrorLog = ex.Message;
        }

        job.CompletedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(job);

        return job;
    }

    public async Task<DmsIngestJob> GetJobAsync(Guid jobId)
    {
        return await _jobRepository.FirstOrDefaultAsync(jobId);
    }

    public async Task<List<DmsIngestJob>> ListJobsAsync(int pageNo, int pageSize)
    {
        if (pageNo < 1)
        {
            pageNo = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        return await (await _jobRepository.GetAllAsync())
            .OrderByDescending(j => j.CreationTime)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    private async Task AddFolderToArchiveAsync(ZipArchive archive, DmsFolder folder, string prefix)
    {
        var documentsInFolder = await DocumentListAsync(folder.Id);

        foreach (var document in documentsInFolder)
        {
            try
            {
                await using var content = await _documentManager.OpenReadAsync(document.Id);

                var entryName = $"{prefix}{folder.Name}/{document.FileName ?? document.Name}";
                var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                await using var entryStream = entry.Open();
                await content.CopyToAsync(entryStream);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Export failed for document {document.Id}: {ex.Message}");
            }
        }

        var children = await _folderManager.GetChildrenAsync(folder.Id);

        foreach (var child in children)
        {
            await AddFolderToArchiveAsync(archive, child, $"{prefix}{folder.Name}/");
        }
    }

    private async Task<List<DmsDocument>> DocumentListAsync(Guid folderId)
    {
        var result = await _documentManager.ListAsync(new DocumentListRequest
        {
            FolderId = folderId,
            PageNo = 1,
            PageSize = int.MaxValue
        });

        return result.Data;
    }

    private async Task<Guid> EnsureFolderPathAsync(Guid rootFolderId, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return rootFolderId;
        }

        var segments = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var currentId = rootFolderId;

        foreach (var segment in segments)
        {
            var children = await _folderManager.GetChildrenAsync(currentId);
            var match = children.FirstOrDefault(c => string.Equals(c.Name, segment, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                currentId = match.Id;
                continue;
            }

            var created = await _folderManager.CreateAsync(new DmsFolder
            {
                Name = segment,
                ParentId = currentId,
                CollectionId = (await _folderManager.GetAsync(currentId)).CollectionId
            });

            currentId = created.Id;
        }

        return currentId;
    }

    private static string GetDirectoryName(string fullName)
    {
        var normalized = fullName.Replace('\\', '/');
        var index = normalized.LastIndexOf('/');

        return index < 0 ? string.Empty : normalized.Substring(0, index);
    }
}
