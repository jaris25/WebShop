using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebShop.Data.Contexts;
using WebShop.Data.Entities;
using WebShop.Data.Repository;
using Xunit;

namespace WebShop.Tests
{
    public class OrderRepositoryTests
    {
        //SharedDatabaseFixture Fixture;
        //public ItemRepositoryTests(SharedDatabaseFixture fixture)
        //{
        //    Fixture = fixture;
        //}

        //  public SharedDatabaseFixture Fixture { get; }

        //[Fact]
        //public void IsItemAvailableReturnTrueIfItemIsInStock()
        //{
        //    using(var transaction = Fixture.Connection.BeginTransaction())
        //    {
        //        using(var context = Fixture.CreateContext(transaction))
        //        {
        //            var repo = new OrdersRepository(context);
        //            var item = context.OrderItems.Where(i => i.Quantity > 3).FirstOrDefault();
        //            Assert.False(repo.ItemIsInStock(item));
        //        }
        //    }
        //}

        [Fact]
        public void IsItemAvailableReturnsFalseIfItemIsNotInStock()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                           .UseInMemoryDatabase(databaseName: $"WebShopDatabase{Guid.NewGuid()}")
                           .Options;

            using (var context = new WebShopContext(options))
            {
                var supply = new Supplier() { Quantity = 3 };
                var product = new Product() { BrandName = "A", ProductSuppliers = new List<ProductSupplier>() { new ProductSupplier { Supplier = supply } } };

                var orderItem = new OrderItem() { Product = product, Quantity = 4 };
                var orderItem2 = new OrderItem() { Product = product, Quantity = 3 };
                var orderItemsList = new List<OrderItem>() { orderItem, orderItem2 };
                var order = new Order() { OrderItems = orderItemsList };

                context.Suppliers.Add(supply);
                context.Products.Add(product);
                context.OrderItems.AddRange(orderItemsList);
                context.Orders.Add(order);

                context.SaveChanges();

                var repo = new OrdersRepository(context);
               
                var supplierInStock = repo.GetAvailableSupplier(orderItem2);
                var supplierOutOfStock = repo.GetAvailableSupplier(orderItem);
                Assert.Null(supplierOutOfStock);
                Assert.NotNull(supplierInStock);
            }
        }

        [Fact]
        public void OrderTwoItemsWhenOneIsOutOfStock()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                           .UseInMemoryDatabase(databaseName: $"WebShopDatabase{Guid.NewGuid()}")
                           .Options;

            using (var context = new WebShopContext(options))
            {
                var supply = new Supplier() { Quantity = 5 };
                var product = new Product() {BrandName = "A", ProductSuppliers = new List<ProductSupplier>() { new ProductSupplier { Supplier = supply} }};
                var orderItem = new OrderItem() { Product = product, Quantity = 4 };
                var orderItem2 = new OrderItem() { Product = product, Quantity = 3 };
                var orderItemsList = new List<OrderItem>() { orderItem, orderItem2 };
                var order = new Order() { OrderItems = orderItemsList };

                context.Suppliers.Add(supply);
                context.Products.Add(product);
                context.OrderItems.AddRange(orderItemsList);
                context.Orders.Add(order);

                context.SaveChanges();

                var repo = new OrdersRepository(context);
                repo.ConfirmOrder(order);
                Assert.Equal(1, supply.Quantity);

            }
        }
    }
}
