using Microsoft.EntityFrameworkCore;

using CoffeBotAPI.Model;

namespace CoffeBotAPI.Data.APIdata
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>()
                .Property(o => o.Status)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }

    }
}