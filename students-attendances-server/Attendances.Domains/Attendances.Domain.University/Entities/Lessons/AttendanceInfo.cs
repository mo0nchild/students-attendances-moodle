using Attendances.Domain.Core.Models;
using Attendances.Domain.University.Entities.Users;

namespace Attendances.Domain.University.Entities.Lessons;

public class AttendanceInfo : BaseEntity
{
    public string Acronym { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    
    public long StudentId { get; set; } = default;
}