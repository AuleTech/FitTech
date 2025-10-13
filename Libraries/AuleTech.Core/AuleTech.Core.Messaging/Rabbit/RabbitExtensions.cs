namespace AuleTech.Core.Messaging.Rabbit;

internal static class RabbitExtensions
{
    public static string GetQueueName<TMessage>()
    {
        return $"{typeof(TMessage).Name}Queue";
    }

    public static string GetDlQueueName<T>()
    {
        return $"{GetQueueName<T>()}_dlq";
    }
}
