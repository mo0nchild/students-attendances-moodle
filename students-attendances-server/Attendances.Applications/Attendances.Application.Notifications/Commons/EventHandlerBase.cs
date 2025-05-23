using System.Reflection;
using Attendances.Application.Notifications.Interfaces;
using Attendances.Domain.Core.MessageBus;

namespace Attendances.Application.Notifications.Commons;

internal abstract class EventHandlerBase : IEventHandler
{
    protected EventHandlerBase() : base() { }
    
    public virtual void RegisterHandlers(IEventMethodDispatcher dispatcher)
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes<EventHandlerAttribute>();
            foreach (var attr in attributes)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 2 || parameters[1].ParameterType != typeof(CancellationToken))
                    throw new InvalidOperationException($"Method {method.Name} must have (payload, CancellationToken)");

                var payloadType = parameters[0].ParameterType;
                var methodCopy = method;

                dispatcher.Register(attr.EventType, async (payloadToken, ct) =>
                {
                    var payload = payloadToken.ToObject(payloadType)!;
                    var result = methodCopy.Invoke(this, new[] { payload, ct });
                    if (result is Task task) await task;
                });
            }
        }
    }
}