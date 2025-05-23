namespace Attendances.MessageBrokers.RabbitMQ.Settings;

public class BrokerBaseSetting
{
    public string Uri { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}