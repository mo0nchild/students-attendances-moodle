using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Notifications.Commons;
using Attendances.Domain.Core.Repositories;

namespace Attendances.System.WebApi.Services.Workers;

public class FetchHostedService : BackgroundService
{
    private readonly IEventMethodDispatcher _eventMethodDispatcher;
    private readonly IRfidMarkerService _rfidMarkerService;
    private readonly IExternalHealthcheckService _externalHealthcheckService;
    private readonly IGlobalFetchService _globalFetchService;
    private readonly InnerTransactionProcessor _transactionProcessor;

    public static readonly Guid FetchTransactionUuid = Guid.NewGuid();

    public FetchHostedService(IGlobalFetchService globalFetchService, 
        [FromKeyedServices("FetchTransaction")] InnerTransactionProcessor transactionProcessor,
        IEventMethodDispatcher eventMethodDispatcher,
        IRfidMarkerService rfidMarkerService,
        IExternalHealthcheckService externalHealthcheckService,
        ILogger<FetchHostedService> logger)
    {
        _globalFetchService = globalFetchService;
        _eventMethodDispatcher = eventMethodDispatcher;
        _rfidMarkerService = rfidMarkerService;
        _transactionProcessor = transactionProcessor;
        _externalHealthcheckService = externalHealthcheckService;
        Logger = logger;
    }
    private ILogger<FetchHostedService> Logger { get; }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_externalHealthcheckService.GetCurrentStatus().IsAvailable)
            {
                Logger.LogError($"Fetching hosted service is not available");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }
            try
            {
                using var _ = await _transactionProcessor.BeginInnerTransaction(FetchTransactionUuid, stoppingToken);
                await _globalFetchService.FetchExternalAsync(stoppingToken);
                _eventMethodDispatcher.UpdateLastFullSyncTime(DateTime.UtcNow);
                
                await _rfidMarkerService.ClearNotUsingRfidMarkersAsync();
            }
            catch (ProcessException error)
            {
                Logger.LogError(error, $"Cannot fetch data from external API: {error.Message}");
                if (error.Type == "notavailable")
                {
                    await _externalHealthcheckService.SetRequestToCheck();
                    continue;
                }
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}

public class SyncSharedContext : ISyncSharedContext
{
    private readonly Dictionary<Type, object> _storage = new();
    
    public void Set<TValue>(TValue value) => _storage[typeof(TValue)] = value!;

    public TValue? Get<TValue>()
    {
        if (_storage.TryGetValue(typeof(TValue), out var value))
            return (TValue)value;

        return default;
    }

    public void Dispose() => _storage.Clear();
}