using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Models.Entities;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//builder.Services.AddMassTransit(configurator =>
//{
//    configurator.AddConsumer<OrderCreatedEventConsumer>();

//    configurator.UsingRabbitMq((context, _configurator) =>
//    {
//        _configurator.Host(builder.Configuration["RabbitMQ"]);
//        // Kuyruk isimleri uygun formatta merkezi yerde tutulmalı
//        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
//    });
//});


//builder.Services.AddScoped<MongoDBService>();


#region MongoDb Seed Data
//İş bittikten sonra imha et
//using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();

//MongoDBService mondoDbService = scope.ServiceProvider.GetService<MongoDBService>();

//var collection = mondoDbService.GetCollection<Stock.API.Models.Entities.Stock>();


//if (!collection.FindSync(s => true).Any())
//{
//    await collection.InsertOneAsync(new() { ProductId = new Guid("51129D62-C168-4AA3-8942-12A11679A503"), Count = 100 });
//    await collection.InsertOneAsync(new() { ProductId = new Guid("C0A4C4C7-E63A-4B5D-B28F-9A497018B9CC"), Count = 3300 });
//    await collection.InsertOneAsync(new() { ProductId = new Guid("FF6D37C5-6358-4A84-A766-3533B3098924"), Count = 4500 });
//    await collection.InsertOneAsync(new() { ProductId = new Guid("A056D79B-4312-4E75-87FD-684E8E1987C4 "), Count = 5871 });
//}
#endregion

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("/ready", () =>
{
    Console.WriteLine("Stock service is ready");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock service is committed");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock service is rollbacked");
});

app.Run();
