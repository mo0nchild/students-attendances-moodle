using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Domain.Core.MessageBus;

public interface IMessageConsumer<in TMessage> where TMessage : MessageBase
{
    public Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}

public static class MessageConsumerRegistator
{
    public static Task Registrate<TConsumer, TMessage>(IServiceCollection collection, string? consumerTag = null, 
        Func<IServiceProvider, TConsumer>? consumerFactory = null)
        where TMessage : MessageBase
        where TConsumer : class, IMessageConsumer<TMessage>
    {
        if (consumerTag != null && consumerFactory != null)
        {
            collection.AddKeyedSingleton<IMessageConsumer<TMessage>, TConsumer>(consumerTag, 
                implementationFactory: (provider, _) => consumerFactory.Invoke(provider));
        }
        else if (consumerTag != null)
        {
            collection.AddKeyedSingleton<IMessageConsumer<TMessage>, TConsumer>(consumerTag);
        }
        else if (consumerFactory != null)
        {
            collection.AddSingleton<IMessageConsumer<TMessage>, TConsumer>(consumerFactory);
        }
        else collection.AddSingleton<IMessageConsumer<TMessage>, TConsumer>();
        return Task.FromResult(collection);
    }
}