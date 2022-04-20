using CAP_RabbitMQ;
using Client;
using Client.Service;
using Demo.Grpc.Protos;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IHelloWorldService, HelloWorldService>();
// Add services to the container.
builder.Services.AddRazorPages();
#region gRPC
builder.Services.AddGrpc(s =>
{
    s.EnableDetailedErrors = true;
    s.MaxReceiveMessageSize = int.MaxValue;
    s.MaxSendMessageSize = int.MaxValue;
});

//client add gRPC service
builder.Services.AddGrpcClient<GHelloWorld.GHelloWorldClient>(opts =>
{
    //这里必须是https ！！！！Server端地址
    opts.Address = new Uri("https://localhost:5267");
    //配置client通道
    opts.ChannelOptionsActions.Add(a =>
    {
        a.MaxReceiveMessageSize = null;
        a.HttpHandler = null;
        a.ThrowOperationCanceledOnCancellation = false;
        a.CompressionProviders = null;
        a.DisableResolverServiceConfig = false;
    });
    //配置监听器   InterceptorScope.Client 可以为每个client端创建一个新的侦听器
}).AddInterceptor<LoggingInterceptor>(/*InterceptorScope.Client*/);

#endregion

#region CAP
//sql Service
builder.Services.AddEntityFrameworkSqlServer().AddDbContext<CAPDBContext>(options =>
{
    options.UseSqlServer("CAPDBConnection").UseLoggerFactory(CAPLoggerFactory.capLoggerFactory);
}, ServiceLifetime.Scoped);

//Config

builder.Services.AddCap(x =>
{
    x.DefaultGroupName = "QueueName";
    x.UseSqlServer((config) => { config.ConnectionString = "CAPDBConnection"; config.Schema = "cap"; });

    x.UseRabbitMQ(
        o =>
        {
            o.HostName = "RabbitMQ Server的地址";
            o.Port = 5672;//RabbitMQ Server的端口号，一般默认为5672
            o.UserName = "连接RabbitMQ Server时的账户名，请优先配置具有Admin权限的user";
            o.Password = "连接RabbitMQ Server时的密码信息";
            o.VirtualHost = "在RabbitMQ Server上注册的client名";
            o.ExchangeName = "RabbitMQ Server注册的Exchange信息，消息会在同exchange内传输";

        });

    x.UseDashboard(o =>
    {
        o.PathMatch = "/cap";
    });
    //常规配置
    x.ConsumerThreadCount = 10;
    x.FailedRetryCount = 5;
    x.FailedRetryInterval = 60;
    //x.FailedThresholdCallback = info.Message.Headers[Headers.CallbackName];
    x.SucceedMessageExpiredAfter = 4 * 3600;
});
builder.Services.AddSingleton<IEventBus, EventBus>();

#endregion




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
IHostApplicationLifetime hostApplicationLifetime = app.Lifetime;
hostApplicationLifetime.ApplicationStarted.Register(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<IHelloWorldService>().HelloWorld_gRPC();
    }
});

app.Run();
