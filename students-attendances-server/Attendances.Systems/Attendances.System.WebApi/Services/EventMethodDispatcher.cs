using System.Collections.Concurrent;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Interfaces;
using Attendances.Domain.Core.MessageBus;
using Attendances.Domain.Core.Models;
using Newtonsoft.Json.Linq;

namespace Attendances.System.WebApi.Services;

internal class EventMethodDispatcher : IEventMethodDispatcher
{
    private readonly Dictionary<string, Func<JToken, CancellationToken, Task>> _handlers = new();
    
    private readonly ConcurrentDictionary<string, List<MessageBase>> _buffers = new();
    private readonly object _lock = new();
    private readonly HashSet<string> _scheduledKeys = new();
    
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
    private DateTime _lastFullSyncTime = DateTime.UtcNow;
    
    public EventMethodDispatcher(IEnumerable<IEventHandler> handlers, ILogger<EventMethodDispatcher> logger)
    {
        Logger = logger;
        foreach (var handler in handlers) handler.RegisterHandlers(this);
    }
    private ILogger<EventMethodDispatcher> Logger { get; }

    public virtual void Register(string eventType, Func<JToken, CancellationToken, Task> handler)
    {
        _handlers[eventType.ToLowerInvariant()] = handler;
    }
    
    public void UpdateLastFullSyncTime(DateTime timestamp)
    {
        _lastFullSyncTime = timestamp;
        Logger.LogInformation($"[SYNC] Last full sync timestamp updated to: {_lastFullSyncTime}");
    }
    
    /*public virtual Task DispatchAsync(MessageBase message, CancellationToken cancellationToken)
    {
        if (_handlers.TryGetValue(message.EventType.ToLowerInvariant(), out var handler))
        {
            return handler(message.Payload, cancellationToken);
        }
        Logger.LogWarning($"Event dispatcher: Unknown event type - {message.EventType}");
        return Task.CompletedTask;
    }*/
    public async Task DispatchAsync(MessageBase message, CancellationToken stoppingToken, Func<Guid, Task>? callback = null)
    {
        var convertedTime = ConvertUnixToUtc(message.TimeStamp);
        if (convertedTime < _lastFullSyncTime.AddMinutes(-1))
        {
            Logger.LogInformation($"[SKIP] Ignoring old event: {message.EventType} at {message.TimeStamp}");
            await (callback?.Invoke(message.EventUuid) ?? Task.CompletedTask);
            return;
        }
        var courseId = message.Payload["course_id"]?.ToString() ?? "nocourse";
        var key = $"{courseId}_{message.TimeStamp}";
        var buffer = _buffers.GetOrAdd(key, _ => new List<MessageBase>());
        
        lock (buffer) { buffer.Add(message); }
        lock (_lock)
        {
            if (!_scheduledKeys.Add(key)) return;
        }
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_delay, stoppingToken);
                if (_buffers.TryRemove(key, out var messages))
                {
                    var mainEvent = SelectMainEvent(messages);
                    if (_handlers.TryGetValue(mainEvent.EventType.ToLowerInvariant(), out var handler))
                    {
                        await handler(mainEvent.Payload, stoppingToken);
                        Logger.LogInformation($"[EVENT] {message.EventType} at {message.TimeStamp}");
                    }
                    else Logger.LogWarning($"[UNKNOWN] Unknown event type: {mainEvent.EventType}");
                        
                    foreach (var item in messages) callback?.Invoke(item.EventUuid);
                }
            }
            catch (ProcessException error)
            {
                Logger.LogError(error, "Error in delayed event dispatch");
                throw;
            }
            finally
            {
                lock (_lock) { _scheduledKeys.Remove(key); }
            }
        }, stoppingToken);
    }
    private static MessageBase SelectMainEvent(List<MessageBase> events)
    {
        return events.OrderBy(item => GetPriority(item.EventType)).ThenByDescending(item => item.TimeStamp).First();
        int GetPriority(string eventType) => eventType.ToLowerInvariant() switch
        {
            var t when t.Contains("course_deleted") => 1,
            var t when t.Contains("group_deleted") => 2,
            var t when t.Contains("user_role_unassigned") => 3,
            var t when t.Contains("deleted") => 4,
            var t when t.Contains("updated") => 5,
            _ => int.MaxValue
        };
    }
    private static DateTime ConvertUnixToUtc(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }
}

public static class EventMethodDispatcherExtensions
{
    public static Task<IServiceCollection> AddEventMethodDispatcher(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IEventMethodDispatcher, EventMethodDispatcher>();
        return Task.FromResult(serviceCollection);
    }
}