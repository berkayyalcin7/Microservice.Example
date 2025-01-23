using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<OrderAPIDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

//builder.Services.AddMassTransit(configurator =>
//{
//    // Consumer'larý burada tanýmlayacaðýz.
//    configurator.AddConsumer<PaymentCompletedEventConsumer>();

//    // Consumer'larý burada tanýmlayacaðýz.
//    configurator.AddConsumer<PaymentFailedEventConsumer>();

//    configurator.AddConsumer<StockNotReservedEventConsumer>();

//    // Burada Endpointlerimizi belirteceðiz.
//    configurator.UsingRabbitMq((context, _configurator) =>
//    {
//        _configurator.Host(builder.Configuration["RabbitMQ"]);

//        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));

//        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));

//        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));


//    });
//});

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Order service is ready");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Order service is committed");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order service is rollbacked");
});

app.Run();
