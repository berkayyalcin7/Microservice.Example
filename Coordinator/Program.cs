using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Coordinator.Services.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<TwoPhaseCommitContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("Order.API", client => client.BaseAddress = new Uri("https://localhost:7247"));
builder.Services.AddHttpClient("Stock.API", client => client.BaseAddress = new Uri("https://localhost:7013"));
builder.Services.AddHttpClient("Payment.API", client => client.BaseAddress = new Uri("https://localhost:7048"));

builder.Services.AddTransient<ITransactionService, TransactionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    // Phase 1 
    var transactionId = await transactionService.CreateTransactionAsync();
    // Servisleri haz�rla
    await transactionService.PrepareServicesAsync(transactionId);
    // Kontrol
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        await transactionService.CommitAsync(transactionId);

        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }
    if (!transactionState)
    {
        await transactionService.RollBackAsync(transactionId);
    }
});

app.Run();
