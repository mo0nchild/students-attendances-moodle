namespace Attendances.Domain.Core.MessageBus;

public interface IMessageProducer
{
    public Task SendAsync<TMessage>(string publishingPath, TMessage message) 
        where TMessage : MessageBase;
    
    public Task SendToAllAsync<TMessage>(string publishingPath, TMessage message)
        where TMessage : MessageBase;
}