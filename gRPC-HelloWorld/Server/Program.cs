using CAP_RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddGrpc(s =>
{
    s.EnableDetailedErrors = true;
    s.MaxReceiveMessageSize = int.MaxValue;
    s.MaxSendMessageSize = int.MaxValue;
});

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

app.UseEndpoints(endpoints =>
{
    //对外暴露gRPC服务
    endpoints.MapGrpcService<HelloWorld>();
});
app.MapRazorPages();

app.Run();
