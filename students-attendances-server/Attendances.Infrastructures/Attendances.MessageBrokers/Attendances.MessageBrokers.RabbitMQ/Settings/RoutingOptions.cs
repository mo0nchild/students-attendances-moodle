namespace Attendances.MessageBrokers.RabbitMQ.Settings;

public class RoutingOptions
{
    public required string QueueName { get; set; }
    public string? ConsumerTag { get; set; }
    public string? ExchangeName { get; set; }
}