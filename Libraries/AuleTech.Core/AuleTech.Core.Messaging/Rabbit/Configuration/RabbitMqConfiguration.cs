namespace AuleTech.Core.Messaging.Rabbit.Configuration;

public class RabbitMqConfiguration
{
    public static string SectionName = "RabbitMq";
    public string ConnectionString { get; set; } = null!;
    public int MaxRetries { get; set; } = 3;
}
