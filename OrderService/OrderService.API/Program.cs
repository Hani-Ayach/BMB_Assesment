using OrderService.Infrastructure.Entities;
using OrderService.Infrastructure;
using Serilog;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()   // logs to console
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var orderContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    orderContext.Orders.AddRange(
        new Order
        {
            ProductId=1,
            Quantity = 2,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            ClientId="1"
        },
        new Order
        {
            ProductId = 1,
            Quantity = 1,
            OrderDate = DateTime.UtcNow,
            ClientId = "1"

        }
    );
    orderContext.SaveChanges();
}

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

