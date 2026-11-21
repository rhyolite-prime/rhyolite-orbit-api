using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace RhyoliteOrbit.DomainServices.Dms;

public class IndexDocumentJobArgs
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }
}

/// <summary>
/// Persisted background job that runs the document pipeline (scan, extract, OCR,
/// AI, embeddings). ABP retries failed jobs automatically.
/// </summary>
public class IndexDocumentJob: BackgroundJob<IndexDocumentJobArgs>, ITransientDependency
{
    private readonly IDocumentPipeline _pipeline;

    public IndexDocumentJob(IDocumentPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public override async Task ExecuteAsync(IndexDocumentJobArgs args)
    {
        await _pipeline.ProcessAsync(args.DocumentId, args.VersionId);
    }
}

public interface IDmsIndexingQueue
{
    Task EnqueueAsync(Guid documentId, Guid versionId);
}

/// <summary>
/// Queues a document for processing using the ABP background job manager and, when
/// a broker is configured, publishes an event for external consumers.
/// </summary>
public class DmsIndexingQueue: IDmsIndexingQueue, ITransientDependency
{
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IDmsNotifier _notifier;

    public DmsIndexingQueue(IBackgroundJobManager backgroundJobManager, IDmsNotifier notifier)
    {
        _backgroundJobManager = backgroundJobManager;
        _notifier = notifier;
    }

    public async Task EnqueueAsync(Guid documentId, Guid versionId)
    {
        await _backgroundJobManager.EnqueueAsync<IndexDocumentJob, IndexDocumentJobArgs>(
            new IndexDocumentJobArgs { DocumentId = documentId, VersionId = versionId });

        await _notifier.PublishAsync("DocumentUploaded", documentId, new { DocumentId = documentId, VersionId = versionId });
    }
}
