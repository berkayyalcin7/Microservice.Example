using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
           return (await _context.NodeState.Where(ns => ns.TransactionId == transactionId).ToListAsync()).TrueForAll(ns=>ns.IsReady==Enums.ReadyType.Ready);
        }

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeState
                .Where(ns=>ns.TransactionId==transactionId)
                .Include(ns=>ns.Node).ToListAsync();

            foreach (var tNode in transactionNodes)
            {
                try
                {
                    var response = await (tNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("commit"),
                        "Stock.API" => _stockHttpClient.GetAsync("commit"),
                        "Payment.API" => _paymentHttpClient.GetAsync("commit"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    // Gelen cevaba göre ilgili servis Tamam mı , İptal mi
                    tNode.TransactionState= result ? Enums.TransactionState.Done: Enums.TransactionState.Abort;

                }
                catch (Exception)
                {
                    tNode.TransactionState= Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
            => (await _context.NodeState.
                    Where(ns => ns.TransactionId == transactionId)
                    .ToListAsync())
                    .TrueForAll(ns=>ns.TransactionState==Enums.TransactionState.Done);

        public async Task RollBackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeState
                .Include(ns=>ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    // Rollback talimatı tamamlananlara verilecek , geri almak için
                    if (transactionNode.TransactionState==Enums.TransactionState.Done)
                    {
                       _ =  await (transactionNode.Node.Name switch
                        {
                            "Order.API" => _orderHttpClient.GetAsync("rollback"),
                            "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                            "Payment.API" => _paymentHttpClient.GetAsync("rollback"),
                        });
                        // İşlem başarısız olduğu için Abort çevir.
                        transactionNode.TransactionState= Enums.TransactionState.Abort;
                    }

                   await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
    }
}
