namespace Attendances.Domain.Core.Repositories;

public interface ISyncProcessor
{
    Task ProcessSyncAsync();
    void SetSharedContext(ISyncSharedContext context);
    
    [AttributeUsage(AttributeTargets.Class)]
    public class SyncOrderAttribute(int order) : Attribute
    {
        public int Order { get; } = order;
    }
}

public interface ISyncSharedContext : IDisposable
{
    void Set<TValue>(TValue value);
    TValue? Get<TValue>();
}