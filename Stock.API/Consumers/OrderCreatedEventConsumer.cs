using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDbService;
        IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;

        public OrderCreatedEventConsumer(MongoDBService mongoDbService, IMongoCollection<Models.Entities.Stock> stockCollection)
        {
            _stockCollection = _mongoDbService.GetCollection <Stock.API.Models.Entities.Stock>();
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();

            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                // Veritabanında var yok kontrolü. olanları Ekle
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());

            }

            Console.Out.WriteLineAsync(context.Message.OrderId + " - " + context.Message.BuyerId);

            //return Task.CompletedTask;
        }
    }
}
