using Microsoft.EntityFrameworkCore;
using UniqloMVC.Models;

namespace UniqloMVC.DataAcces
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }

        public UniqloDbContext(DbContextOptions opt) : base(opt) { }
    }
}
