using BmbPlatform.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Entities;
using System.Linq.Dynamic.Core;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductDbContext> _logger;

        private readonly ProductDbContext _context;
        public ProductsController(ProductDbContext context, ILogger<ProductDbContext> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            try
            {

                var query = _context.Products.AsQueryable();

                var totalCount = await query.CountAsync();

                var products = await query
                    .OrderBy(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToListAsync();

                var result = new
                {
                    Items = products,
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
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order Created Successfully");
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return BadRequest();

        }
    }
}
