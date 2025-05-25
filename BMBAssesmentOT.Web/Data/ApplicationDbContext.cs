using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BMBAssesmentOT.Web;

namespace BMBAssesmentOT.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BMBAssesmentOT.Web.Order> Order { get; set; } = default!;
    }
}
