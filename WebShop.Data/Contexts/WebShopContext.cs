using Microsoft.EntityFrameworkCore;
using WebShop.Data.Entities;

namespace WebShop.Data.Contexts
{
    public class WebShopContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Discount> Discoutns { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RegistrationDetails> RegistrationDetails { get; set; }
        public WebShopContext(DbContextOptions<WebShopContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.RegistrationDetails)
                .WithOne(r => r.Customer)
                .IsRequired();

            modelBuilder.Entity<Discount>()
                .HasMany(d => d.OrderItems)
                .WithOne(o => o.Discount)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
