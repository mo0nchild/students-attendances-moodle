using Attendances.Application.Manager.Models.CommonModels;

namespace Attendances.Application.Manager.Interfaces;

public interface IRfidMarkerService
{
    Task<IReadOnlyList<RfidMarkerModel>> GetAllRfidMarkersAsync();
    
    Task<IReadOnlyList<RfidMarkerDetailModel>> GetAllRfidMarkerDetailsAsync();
    Task<IReadOnlyList<RfidMarkerModel>> GetRfidMarkersByCourseIdAsync(long courseId);
    
    Task<IReadOnlyList<UserInfoModel>> GetUsersWithoutRfidMarkersAsync();
    
    Task SetRfidMarkersAsync(NewRfidMarkerModel rfidMarker);
    Task DeleteRfidMarkersAsync(Guid rfidMarkerUuid);
    Task ClearNotUsingRfidMarkersAsync();
}