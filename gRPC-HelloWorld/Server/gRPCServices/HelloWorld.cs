
using Demo.Grpc.Protos;
using Grpc.Core;

namespace Server
{
    public class HelloWorld : GHelloWorld.GHelloWorldBase
    {
        public HelloWorld()
        {

        }
        //实现gRPC服务 GetHelloWorld 方法
        public override Task<HelloWorldReturnMsg> GetHelloWorld(HelloWorldMsg request, ServerCallContext context)
        {
            var msg = new HelloWorldReturnMsg() { Context = "Hello World!", Status = true };
            return Task.FromResult(msg);
        }
    }
}
