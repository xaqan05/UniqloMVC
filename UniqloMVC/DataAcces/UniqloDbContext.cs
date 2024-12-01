using Microsoft.EntityFrameworkCore;
using UniqloMVC.Models;

namespace UniqloMVC.DataAcces
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }
        public UniqloDbContext(DbContextOptions opt) : base(opt) { }
    }
}
