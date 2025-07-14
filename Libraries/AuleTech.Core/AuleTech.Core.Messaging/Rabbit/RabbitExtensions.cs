namespace AuleTech.Core.Messaging.Rabbit;

internal static class RabbitExtensions
{
    public static string GetQueueName<TMessage>() => $"{typeof(TMessage).Name}Queue";
    public static string GetDlQueueName<T>() => $"{GetQueueName<T>()}_dlq";
}
