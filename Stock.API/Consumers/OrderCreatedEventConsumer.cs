using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDbService;
        IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;
        // MassTransit
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDBService mongoDbService, IMongoCollection<Models.Entities.Stock> stockCollection, ISendEndpointProvider sendEndpointProvider)
        {
            _stockCollection = _mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();

            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                // Veritabanında var yok kontrolü. olanları Ekle
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());

            }

            // Tüm değerler odğru ise - Tutarlı bir sipariş
            if (stockResult.TrueForAll(sr=>sr.Equals(true)))
            {
                // Gerekli sipariş işlemleri
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                    Stock.API.Models.Entities.Stock stock = await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;

                    // Mongo DB için replace işlemi
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
                }

                // Stok işlemleri tamamlandığında Payment'a göndermemiz gerekiyor. (Bir kuyruğa özel hedefli bir şekilde Asenkron olarak yayınlayacağız.)
                // Publish yerine Send yöntemiyle yayınlayacağız.
                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId=context.Message.BuyerId,
                    OrderId=context.Message.OrderId,
                    TotalPrice=context.Message.TotalPrice,
                };

                // Hangi Endpoint'e gönderileceği
                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                // Gönderme işlemi
                await sendEndpoint.Send(stockReservedEvent);

                Console.Out.WriteLine("Stock İşlemleri Başarılı");


            }
            else
            {
                // Sipariş geçersiz olduğuna dair işlemler
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "...",
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);


                Console.Out.WriteLine("Stock İşlemleri Başarısız.");
            }


            Console.Out.WriteLineAsync(context.Message.OrderId + " - " + context.Message.BuyerId);

            //return Task.CompletedTask;
        }
    }
}
