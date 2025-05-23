using Attendances.Application.Notifications.Models;

namespace Attendances.Application.Notifications.Interfaces;

public interface IEventExternalCache
{
    Task<IReadOnlyList<EventInfoModel>> GetAllEvents();
    Task AddEvent(EventInfoModel eventInfo);
    
    Task CompleteEvent(Guid eventUuid);
}