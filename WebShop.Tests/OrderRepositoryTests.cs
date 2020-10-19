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
        [Fact]
        public void IsItemAvailableReturnsNullIfItemIsNotInStock()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                           .UseInMemoryDatabase(databaseName: $"WebShopDatabase{Guid.NewGuid()}")
                           .Options;

            using (var context = new WebShopContext(options))
            {
                var supply = new Supplier() { Quantity = 3 };
                var product = new Product() { BrandName = "A", ProductSuppliers = new List<ProductSupplier>() { new ProductSupplier { Supplier = supply } } };
                var orderItem = new OrderItem() { Product = product, Quantity = 4 };
                var repo = new OrdersRepository(context);

                var supplierOutOfStock = repo.GetAvailableSupplier(orderItem);
                Assert.Null(supplierOutOfStock);
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
                repo.ConfirmOrder(order);

                Assert.Equal(1, supply.Quantity);
            }
        }

        [Fact]
        public void CheckIfItemWasRemovedFromOrderWhenThereAreNotEnoughItemsInStock()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                          .UseInMemoryDatabase(databaseName: $"WebShopDatabase{Guid.NewGuid()}")
                          .Options;

            using (var context = new WebShopContext(options))
            {
                var supply = new Supplier() { Quantity = 5 };
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
                repo.ConfirmOrder(order);

                Assert.Single(order.OrderItems.ToList());
            }
        }
    }
}
