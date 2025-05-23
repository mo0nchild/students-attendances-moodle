using Attendances.Domain.Core.Repositories;
using Attendances.Domain.Messages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Domain.Messages.Repositories;

public interface IEventRepository : IBaseRepository
{
    DbSet<EventInfo> EventInfos { get; set; }
}