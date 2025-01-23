using MassTransit;
using Payment.API.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddMassTransit(configurator =>
//{
//    configurator.AddConsumer<StockReservedEventConsumer>();

//    configurator.UsingRabbitMq((context, _configurator) =>
//    {
//        _configurator.Host(builder.Configuration["RabbitMQ"]);
//        // Kuyruk isimleri uygun formatta merkezi yerde tutulmalý
//        _configurator.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue, e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
//    });
//});

var app = builder.Build();


app.MapGet("/ready", () =>
{
    Console.WriteLine("Payment service is ready");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Payment service is committed");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Payment service is rollbacked");
});


app.Run();
