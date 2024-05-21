using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _dbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            // Değişiklikleri al ve veritabanına yansıt.
            Order.API.Models.Entities.Order order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus=Models.Enums.OrderStatus.Completed;

            await _dbContext.SaveChangesAsync();    
        }
    }
}
