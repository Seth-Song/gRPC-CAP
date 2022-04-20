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

app.UseEndpoints(endpoints =>
{
    //���Ⱪ¶gRPC����
    endpoints.MapGrpcService<HelloWorld>();
});
app.MapRazorPages();

app.Run();
