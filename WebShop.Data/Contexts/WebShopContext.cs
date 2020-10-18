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

            // Some dicounts may be applied to many different products, after removing the discount product should stay
            modelBuilder.Entity<Discount>()
                .HasMany(d => d.Products)
                .WithOne(o => o.Discount)
                .OnDelete(DeleteBehavior.Restrict);

            // On deleting product delete behavior should be restricted because after removing product from sales some orders with this product may not be fulfilled yet.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(i => i.Product)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
