using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    }
}
