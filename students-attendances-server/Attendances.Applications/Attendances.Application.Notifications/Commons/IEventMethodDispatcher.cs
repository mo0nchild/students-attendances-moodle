using Attendances.Domain.Core.MessageBus;
using Newtonsoft.Json.Linq;

namespace Attendances.Application.Notifications.Commons;

public interface IEventMethodDispatcher
{
    void Register(string eventType, Func<JToken, CancellationToken, Task> handler);
    Task DispatchAsync(MessageBase message, CancellationToken cancellationToken, Func<Guid, Task>? callback = null);
    void UpdateLastFullSyncTime(DateTime timestamp);
}