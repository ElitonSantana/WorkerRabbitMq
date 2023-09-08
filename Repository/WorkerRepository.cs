using ConsumerRabbitMq.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsumerRabbitMq.Repository
{
    public class WorkerRepository
    {
        public async Task SendMessage(string QueueName, string Body)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7115/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var contentString = new StringContent(Body, Encoding.UTF8, "application/json");

                switch (QueueName)
                {
                    case QueueNameConstant.CreateProductQueue:
                        var response = await client.PostAsync("api/Products/Create", contentString);
                        break;
                    case QueueNameConstant.CreateUserQueue:
                        Console.WriteLine("OK Usuário");
                        break;
                    case QueueNameConstant.EmailQueue:
                        Console.WriteLine("OK Email");
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
