﻿using Shared.Events.Common;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class OrderCreatedEvent : IEvent
    {
        // Servisler arası veriyi barındıracak format.
        public Guid OrderId {  get; set; }
        public Guid BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        // Bunu Payment'a göndereceğiz.
        public decimal TotalPrice { get; set; }
    }
}
