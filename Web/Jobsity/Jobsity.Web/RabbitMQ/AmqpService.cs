using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Jobsity.Web
{
    public class AmqpService
    {
        private readonly CloudAMQP _amqp;
        private readonly ConnectionFactory connectionFactory;
        private const string QueueName = "JobsityQueue";

        public AmqpService(IOptions<CloudAMQP> ampOptions)
        {
            _amqp = ampOptions.Value ?? throw new ArgumentNullException(nameof(ampOptions));

            connectionFactory = new ConnectionFactory
            {
                UserName = _amqp.Username,
                Password = _amqp.Password,
                VirtualHost = _amqp.VirtualHost,
                HostName = _amqp.HostName,
                Uri = new Uri(_amqp.Uri)
            };
        }

        public bool PublishMessage(object message)
        {
            try
            {
                using (var conn = connectionFactory.CreateConnection())
                {
                    using (var channel = conn.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: QueueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );

                        var jsonPayload = JsonConvert.SerializeObject(message);
                        var body = Encoding.UTF8.GetBytes(jsonPayload);

                        channel.BasicPublish(exchange: "",
                            routingKey: QueueName,
                            basicProperties: null,
                            body: body
                        );
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
