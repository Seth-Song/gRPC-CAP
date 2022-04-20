using CAP_RabbitMQ;
using DotNetCore.CAP;
using Newtonsoft.Json;
using System.Text.Json;

namespace Server.Event
{
    public class HelloWorldConsumer : ICapSubscribe
    {
        [CapSubscribeAttribute(CAPConstants.HelloWorldKey, false)]
        public async Task<string> Consume(JsonElement msg)
        {
            try
            {
                Console.WriteLine("Hello World");

            }
            catch (Exception e)
            {
                //logger
            }
            return CAPConstants.HelloWorldKey;
        }
    }
}
