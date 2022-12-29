namespace UOC.SharedContracts
{
    public class MessagingOptions
    {
        public MessagingRabbitMQUriBuilder RabbitConfiguration { get; private set; }
        public QueueConfiguration QueueConfiguration { get; private set; }
        public SagaConfiguration SagaConfiguration { get; private set; }
        public MessageConfiguration MessageConfiguration { get; private set; }
    }

    public class QueueConfiguration
    {
        public string OrderSubmit { get; private set; }
        public string OrderPayment { get; private set; }
        public string OrderConfirm { get; private set; }
        public string OrderCancel { get; private set; }
    }

    public class SagaConfiguration
    {
        public string SagaPattern { get; private set; }
    }

    public class MessageConfiguration
    {
        public string MessagePattern { get; private set; }
    }
}