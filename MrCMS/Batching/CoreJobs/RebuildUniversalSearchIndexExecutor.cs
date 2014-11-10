using System;
using MrCMS.Search;
using MrCMS.Services;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildUniversalSearchIndexExecutor : BaseBatchJobExecutor<RebuildUniversalSearchIndex>
    {
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public RebuildUniversalSearchIndexExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IUniversalSearchIndexManager universalSearchIndexManager)
            : base(setBatchJobJobExecutionStatus)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildUniversalSearchIndex batchJob)
        {
            try
            {
                _universalSearchIndexManager.ReindexAll();
                return BatchJobExecutionResult.Success();
            }
            catch (Exception exception)
            {
                return BatchJobExecutionResult.Success(exception.Message);
            }
        }
    }
    public class RebuildLuceneIndexExecutor : BaseBatchJobExecutor<RebuildLuceneIndex>
    {
        private readonly IIndexService _indexService;

        public RebuildLuceneIndexExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IIndexService indexService)
            : base(setBatchJobJobExecutionStatus)
        {
            _indexService = indexService;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildLuceneIndex batchJob)
        {
            try
            {
                _indexService.Reindex(batchJob.IndexName);
                return BatchJobExecutionResult.Success();
            }
            catch (Exception exception)
            {
                return BatchJobExecutionResult.Success(exception.Message);
            }
        }
    }
}