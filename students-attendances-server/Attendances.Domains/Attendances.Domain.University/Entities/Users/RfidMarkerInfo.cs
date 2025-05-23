using Attendances.Domain.Core.Models;

namespace Attendances.Domain.University.Entities.Users;

public class RfidMarkerInfo : BaseEntity 
{
    public long UserId { get; set; } = default;
    public string RfidValue { get; set; } = string.Empty;
}