using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Concrete
{
    public class TransactionService(TwoPhaseCommitContext _context,IHttpClientFactory _httpClientFactory) : ITransactionService
    {

        HttpClient _orderHttpClient= _httpClientFactory.CreateClient("Order.API");
        HttpClient _stockHttpClient = _httpClientFactory.CreateClient("Stock.API");
        HttpClient _paymentHttpClient= _httpClientFactory.CreateClient("Payment.API");
        
        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Nodes.ToListAsync();

            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady=Enums.ReadyType.Pending,
                    TransactionState=Enums.TransactionState.Pending,
                }
            });

            await _context.SaveChangesAsync();
            return transactionId;

        }
        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeState.Include(x => x.Node)
                .Where(ns => ns.TransactionId == transactionId).ToListAsync();

            foreach (var node in transactionNodes)
            {
                // Node kontrolü try catch. Hata durumunda veya Nodelardan biri hazır değil ise kolon UnReady ' ye çekilecek. Transaction başarısız olarak değerlendirilecek.
                try
                {
                    var response = await (node.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    // Gelen cevaba göre ilgili servis hazır mı değil mi cevabı.
                    node.IsReady= result ?  Enums.ReadyType.Ready : Enums.ReadyType.NotReady;

                }
                catch (Exception)
                {
                    node.IsReady= Enums.ReadyType.NotReady;
                }

            }

            await _context.SaveChangesAsync();
        }
        public Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task RollBackAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
