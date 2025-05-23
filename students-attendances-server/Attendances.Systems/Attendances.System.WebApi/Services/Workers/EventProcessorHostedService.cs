using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Interfaces;
using Attendances.Domain.Core.MessageBus;
using AutoMapper;

namespace Attendances.System.WebApi.Services.Workers;

public class EventProcessorHostedService : BackgroundService
{
    private readonly IEventMethodDispatcher _eventMethodDispatcher;
    private readonly IEventExternalCache _eventExternalCache;
    private readonly IMapper _mapper;
    private readonly InnerTransactionProcessor _transactionProcessor;
    private readonly IExternalHealthcheckService _externalHealthcheckService;

    public EventProcessorHostedService(IEventMethodDispatcher eventMethodDispatcher,
        IEventExternalCache eventExternalCache,
        IMapper mapper,
        [FromKeyedServices("FetchTransaction")] InnerTransactionProcessor transactionProcessor,
        IExternalHealthcheckService externalHealthcheckService,
        ILogger<EventProcessorHostedService> logger)
    {
        Logger = logger;
        _eventMethodDispatcher = eventMethodDispatcher;
        _eventExternalCache = eventExternalCache;
        
        _mapper = mapper;
        _transactionProcessor = transactionProcessor;
        _externalHealthcheckService = externalHealthcheckService;
    }
    private ILogger<EventProcessorHostedService> Logger { get; }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_externalHealthcheckService.GetCurrentStatus().IsAvailable)
                {
                    var transactionUuid = FetchHostedService.FetchTransactionUuid;
                    using var _ = await _transactionProcessor.BeginInnerTransaction(transactionUuid, stoppingToken);
                    
                    var processingEvents = await _eventExternalCache.GetAllEvents();
                    foreach (var @event in processingEvents)
                    {
                        await _eventMethodDispatcher.DispatchAsync(_mapper.Map<MessageBase>(@event), stoppingToken,
                            callback: async uuid =>
                            {
                                await _eventExternalCache.CompleteEvent(uuid);
                            });
                    }
                }
            }
            catch (ProcessException error)
            {
                Logger.LogError(error.Message, $"Error occured while processing events: {error.Message}");
                if (error.Type == "notavailable")
                {
                    await _externalHealthcheckService.SetRequestToCheck();
                }
                throw;
            }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}