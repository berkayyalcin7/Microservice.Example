using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _dbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            // Değişiklikleri al ve veritabanına yansıt.
            Order.API.Models.Entities.Order order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = Models.Enums.OrderStatus.Failed;

            await _dbContext.SaveChangesAsync();
        }
    }
}
