using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Application.Sync.Interfaces;
using Attendances.Domain.Core.Repositories;

namespace Attendances.System.WebApi.Services.Workers;

public class SyncProcessorHostedService : BackgroundService
{
    private readonly ILessonSyncProcessor _lessonSyncProcessor;
    private readonly IExternalHealthcheckService _externalHealthcheckService;
    private readonly InnerTransactionProcessor _transactionProcessor;
    
    public SyncProcessorHostedService(ILessonSyncProcessor lessonSyncProcessor,
        IExternalHealthcheckService externalHealthcheckService,
        [FromKeyedServices("FetchTransaction")] InnerTransactionProcessor transactionProcessor, 
        ILogger<SyncProcessorHostedService> logger)
    {
        Logger = logger;
        _lessonSyncProcessor = lessonSyncProcessor;
        _externalHealthcheckService = externalHealthcheckService;
        _transactionProcessor = transactionProcessor;
    }
    private ILogger<SyncProcessorHostedService> Logger { get; }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var syncType = _externalHealthcheckService.GetCurrentStatus().IsAvailable
                    ? SyncProcessingType.Global
                    : SyncProcessingType.Local;
                
                var transactionUuid = FetchHostedService.FetchTransactionUuid;
                using var _ = await _transactionProcessor.BeginInnerTransaction(transactionUuid, stoppingToken);

                await _lessonSyncProcessor.ProcessAsync(syncType, stoppingToken);
                
            }
            catch (ProcessException error)
            {
                Logger.LogError(error.Message, "An unexpected error occured while running the sync processor.");
                if (error.Type == "notavailable")
                {
                    await _externalHealthcheckService.SetRequestToCheck();
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}