using Microsoft.EntityFrameworkCore;
using ProductService.API.Controllers;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Entities;
using Serilog;

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


builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));

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




using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    context.Products.AddRange(
        new Product { Name = "Keyboard", Price = 50 },
        new Product { Name = "Mouse", Price = 25 }
    );
    context.SaveChanges();
}
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();


