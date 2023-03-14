using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop1ConsumerConsole
{
    public class RabbitMQConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public RabbitMQConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            var queueName = new string[] { "QueueA", "QueueB", "QueueC" };

            foreach (var queue in queueName)
            {
                _channel.QueueDeclare(queue: queue,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
                _consumer = new EventingBasicConsumer(_channel);

                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var exchangeName = ea.Exchange;
                    var routingKey = ea.RoutingKey;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0} , RoutingKey = {1} , Exchange = {2}", message,routingKey,exchangeName);
                };
                _channel.BasicConsume(queue: queue,
                                      autoAck: true,
                                      consumer: _consumer);
                Console.ReadLine();
            }
        }
    }
}