using System;

namespace UOC.SharedContracts
{
	public class MessagingRabbitMQUriBuilder
    {
        public string Host { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string VirtualHost { get; private set; }
        public string Scheme { get; private set; }
        public int Port { get; private set; }

        public Uri BuildUri()
        {
            return new Uri($"{Scheme}://{User}:{Password}@{Host}:{Port}/{VirtualHost}");
        }
    }
}
