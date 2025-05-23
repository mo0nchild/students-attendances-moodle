using Attendances.Domain.Core.Repositories;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Domain.University.Repositories;

public interface IUniversityRepository : IBaseRepository
{
    DbSet<AccountInfo> Accounts { get; set; }
    DbSet<UserInfo> Users { get; set; }
    
    DbSet<CourseInfo> Courses { get; set; }
    DbSet<GroupInfo> Groups { get; set; }
    DbSet<LessonInfo> Lessons { get; set; }
    DbSet<RfidMarkerInfo> RfidMarkers { get; set; }
}