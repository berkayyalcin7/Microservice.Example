using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class RabbitMQSettings
    {
        public const string Stock_OrderCreatedEventQueue = "stock-order-created-event";

        public const string Payment_StockReservedEventQueue = "payment-stock-reserved-event";

        public const string Order_PaymentCompletedEventQueue = "order-payment-completed-event";

        public const string Order_StockNotReservedEventQueue = "order-stock-not-reserved-event";
    }
}
