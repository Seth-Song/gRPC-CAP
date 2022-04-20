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
    //���������https ��������Server�˵�ַ
    opts.Address = new Uri("https://localhost:5267");
    //����clientͨ��
    opts.ChannelOptionsActions.Add(a =>
    {
        a.MaxReceiveMessageSize = null;
        a.HttpHandler = null;
        a.ThrowOperationCanceledOnCancellation = false;
        a.CompressionProviders = null;
        a.DisableResolverServiceConfig = false;
    });
    //���ü�����   InterceptorScope.Client ����Ϊÿ��client�˴���һ���µ�������
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
            o.HostName = "RabbitMQ Server�ĵ�ַ";
            o.Port = 5672;//RabbitMQ Server�Ķ˿ںţ�һ��Ĭ��Ϊ5672
            o.UserName = "����RabbitMQ Serverʱ���˻��������������þ���AdminȨ�޵�user";
            o.Password = "����RabbitMQ Serverʱ��������Ϣ";
            o.VirtualHost = "��RabbitMQ Server��ע���client��";
            o.ExchangeName = "RabbitMQ Serverע���Exchange��Ϣ����Ϣ����ͬexchange�ڴ���";

        });

    x.UseDashboard(o =>
    {
        o.PathMatch = "/cap";
    });
    //��������
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
