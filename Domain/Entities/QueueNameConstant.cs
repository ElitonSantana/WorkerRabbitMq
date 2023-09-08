using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerRabbitMq.Domain.Entities
{
    public class QueueNameConstant
    {
        public const string CreateProductQueue = "CreateProductQueue";
        public const string CreateUserQueue = "CreateUserQueue";
        public const string EmailQueue = "EmailQueue";
    }
}
