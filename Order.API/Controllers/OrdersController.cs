using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        readonly OrderAPIDbContext _context;
        // MassTransit üzerinden bir eventi publish edecek instance getirecek.
        readonly IPublishEndpoint _publishEndopint;

        public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndopint)
        {
            _context = context;
            _publishEndopint = publishEndopint;
        }

        /// <summary>
        /// Herhangi bir tasarım deseni kullanmadan buradan işlemleri yapıyoruz.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderVm vm)
        {
            Order.API.Models.Entities.Order order = new()
            {
                OrderId = Guid.NewGuid(),
                BuyerId = vm.BuyerId,
                CreatedDate = DateTime.Now,
                OrderStatus = Models.Enums.OrderStatus.Suspend

            };

            order.OrderItems = vm.OrderItems.Select(x => new OrderItem
            {
                Count = x.Count,
                Price = x.Price,
                ProductId = x.ProductId
            }).ToList();

            order.TotalPrice = vm.OrderItems.Sum(x => (x.Price*x.Count));


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Nesneyi oluşturduk.
            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = order.BuyerId,
                OrderId = order.OrderId,
                OrderItems = order.OrderItems.Select(x => new OrderItemMessage
                {
                    Count = x.Count,
                    ProductId = x.ProductId
                }).ToList(),
                TotalPrice=order.TotalPrice
            };

             // Mass Transit 
             await _publishEndopint.Publish(orderCreatedEvent);


            return Ok();

        }
    }
}
