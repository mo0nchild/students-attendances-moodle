using Attendances.Domain.Core.Repositories;
using Attendances.Domain.Sync.Entities;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Domain.Sync.Repositories;

public interface ISyncRepository : IBaseRepository
{
    DbSet<LessonSyncItem> LessonSyncItems { get; set; }
}