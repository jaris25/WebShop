using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using WebShop.Data.Contexts;
using WebShop.Data.Entities;

namespace WebShop.Tests
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedDatabaseFixture()
        {
            Connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WebShop;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            Seed();

            Connection.Open();
        }

        public DbConnection Connection { get; }

        public WebShopContext CreateContext(DbTransaction transaction = null)
        {
            var context = new WebShopContext(new DbContextOptionsBuilder<WebShopContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var supply = new Supplier() { Quantity = 3 };

                        var product = new Product() { Suppliers = new List<Supplier>() { supply } };
                        var orderItem = new OrderItem() { Product = product, Quantity = 4 };
                        var orderItem2 = new OrderItem() { Product = product, Quantity = 3 };
                        var orderItemsList = new List<OrderItem>() { orderItem, orderItem2 };
                        var order = new Order() { OrderItems = orderItemsList };


                        context.Products.Add(product);
                        context.Suppliers.Add(supply);
                        context.OrderItems.AddRange(orderItemsList);
                        context.Orders.Add(order);

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
