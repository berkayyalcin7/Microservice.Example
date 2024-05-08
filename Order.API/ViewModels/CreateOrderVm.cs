namespace Order.API.ViewModels
{
    public class CreateOrderVm
    {
        public Guid BuyerId { get; set; }
        public List<CreateOrderItemVm> OrderItems { get; set; }
    }

   
}
