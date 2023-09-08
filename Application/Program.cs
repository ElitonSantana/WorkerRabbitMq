using System.Text;
using ConsumerRabbitMq.Domain.Entities;
using ConsumerRabbitMq.Repository;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumerRabbitMq.Application
{
    class Program
    {
        static void Main()
        {
            WorkerRepository workerRepository = new WorkerRepository();

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            #region DeclareQueueForCreates On Console Start.

            List<string> QueueNames = new List<string>()
            {
                QueueNameConstant.CreateProductQueue,
                QueueNameConstant.CreateUserQueue,
                QueueNameConstant.EmailQueue
            };

            foreach (var queueName in QueueNames)
            {
                channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);
            }

            #endregion

            

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting messages.");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Process Queue {ea.RoutingKey} body {message}");

                    await workerRepository.SendMessage(ea.RoutingKey, message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Worker {ex.Message}");
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            foreach (var queueName in QueueNames)
            {
                channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}