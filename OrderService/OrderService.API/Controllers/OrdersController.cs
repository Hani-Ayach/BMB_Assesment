using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Infrastructure.Entities;
using OrderService.Infrastructure;
using BmbPlatform.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        private readonly OrderDbContext _context;
        public OrdersController(OrderDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Orders.AsQueryable();

                var totalCount = await query.CountAsync();

                var orders = await query
                    .OrderBy(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        ProductId = o.ProductId,
                        Quantity = o.Quantity,
                        OrderDate = o.OrderDate
                    })
                    .ToListAsync();

                var result = new
                {
                    Items = orders,
                    RowCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order Created Successfully");
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return BadRequest();

        }

    }
}
