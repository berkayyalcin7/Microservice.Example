using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;
public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {

            // Ödeme işlemleri yapılacak.

            if (true)
            {
                // Ödemenin başarıyla tamamlandığını ifade etmek
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent
                {
                    OrderId=context.Message.OrderId,
                };


                _publishEndpoint.Publish(paymentCompletedEvent);

                Console.WriteLine("Ödeme Başarılı ....");

            }
            else
            {
                // Ödemede sıkıntı olduğu.
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message="Ödeme Başarısız oldu."
                };

                _publishEndpoint.Publish(paymentFailedEvent);

                Console.WriteLine("Ödeme Başarısız ....");
            }




            return Task.CompletedTask;
        }
    }
}
