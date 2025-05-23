using Attendances.Domain.Core.Repositories;

namespace Attendances.Domain.Core.Factories;

public class RepositoryFactoryInterface<TRepository> where TRepository : IBaseRepository
{
    public delegate Task<TRepository> RepositoryFactoryAsync();
    public delegate TRepository RepositoryFactory();

    public RepositoryFactory CreateRepository { get; set; } = default!;
    public RepositoryFactoryAsync CreateRepositoryAsync { get; set; } = default!;
}

public delegate RepositoryFactoryInterface<TRepository> RepositoryFactory<TRepository>() 
    where TRepository : IBaseRepository;
