using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        readonly OrderAPIDbContext _dbContext;

        public StockNotReservedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            // Değişiklikleri al ve veritabanına yansıt.
            Order.API.Models.Entities.Order order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = Models.Enums.OrderStatus.Failed;

            await _dbContext.SaveChangesAsync();
        }
       
    }
}
