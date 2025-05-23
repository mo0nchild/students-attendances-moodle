using System.Collections.Immutable;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Notifications.Interfaces;
using Attendances.Application.Notifications.Models;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Messages.Entities;
using Attendances.Domain.Messages.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Notifications.Services;

internal class EventExternalCache : IEventExternalCache
{
    private readonly RepositoryFactoryInterface<IEventRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly InnerTransactionProcessor _transactionProcessor;
    
    private static readonly Guid SessionGuid = Guid.NewGuid();

    public EventExternalCache(RepositoryFactoryInterface<IEventRepository> repositoryFactory,
        IMapper mapper,
        [FromKeyedServices("ExternalEventTransaction")] InnerTransactionProcessor transactionProcessor,
        ILogger<EventExternalCache> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _transactionProcessor = transactionProcessor;
    }
    private ILogger<EventExternalCache> Logger { get; }
    
    public async Task<IReadOnlyList<EventInfoModel>> GetAllEvents()
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        using var _ = await _transactionProcessor.BeginInnerTransaction(SessionGuid);

        var eventsList = await dbContext.EventInfos.Where(item => !item.IsHandled).ToListAsync();
        return _mapper.Map<List<EventInfoModel>>(eventsList);
    }

    public async Task AddEvent(EventInfoModel eventInfo)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        await dbContext.EventInfos.AddRangeAsync(new EventInfo()
        {
            Payload = eventInfo.Payload,
            TimeStamp = eventInfo.TimeStamp,
            EventType = eventInfo.EventType,
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task CompleteEvent(Guid eventUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        using var _ = await _transactionProcessor.BeginInnerTransaction(SessionGuid);
        
        var eventInfo = await dbContext.EventInfos.FirstOrDefaultAsync(item => item.Uuid == eventUuid);
        if (eventInfo != null)
        {
            eventInfo.IsHandled = true;
            await dbContext.SaveChangesAsync();
        } 
    }
}