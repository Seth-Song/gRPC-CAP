using CAP_RabbitMQ;
using Client.Service;
using Demo.Grpc.Protos;

namespace Client
{
    public class HelloWorldService: IHelloWorldService
    {
        private readonly GHelloWorld.GHelloWorldClient _gHelloWorldClient;
        private readonly IEventBus _eventBus;
        public HelloWorldService(GHelloWorld.GHelloWorldClient gHelloWorldClient, IEventBus eventBus)
        {
            _gHelloWorldClient = gHelloWorldClient;
            _eventBus = eventBus;
        }

        public void HelloWorld_CAP()
        {
            try
            {
                var msg = "Hello World";
                var key = CAPConstants.HelloWorldKey;
                _eventBus.Publish(msg, key);
            }
            catch (Exception e)
            {
                //logger.Error
                throw;
            }
        }

        public string HelloWorld_gRPC()
        {         
            try
            {
               return _gHelloWorldClient.GetHelloWorld(new HelloWorldMsg() { Id = "hello world id" }).Context;
            }
            catch (Exception e)
            {
                //logger.Error
                throw;
            }
        }
    }
}
