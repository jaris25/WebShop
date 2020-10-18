using System;
using System.Linq;
using WebShop.Data.Contexts;
using WebShop.Data.Entities;

namespace WebShop.Data.Repository
{
    public class OrdersRepository: IOrdersRepository
    {
        private readonly WebShopContext _context; 
        private readonly object supplyLock = new object();

        public OrdersRepository(WebShopContext context)
        {
            _context = context;
        }

        public void ConfirmOrder(Order order)
        {
            foreach(var item in order.OrderItems)
            {
                // Additional check to see if item is still available when confirming order.
                if (!ItemIsInStock(item))
                {
                    continue;
                }
                var supplier = item.Product.Suppliers.Where(s => s.Quantity >= item.Quantity).FirstOrDefault();
                lock (supplyLock)
                {
                    supplier.Quantity -= item.Quantity;
                }
            }
        }

        public void AddItemToOrder(OrderItem orderItem, Order order)
        {
            if (ItemIsInStock(orderItem))
            {
                order.OrderItems.Add(orderItem);
            }
            _context.Orders.Add(order);
        }

        public bool ItemIsInStock(OrderItem item)
        {
            return item.Product.Suppliers.Where(s => s.Quantity >= item.Quantity).FirstOrDefault() != null ? true : false;
        }


        public void countPrice(Order order)
        {
            foreach(var item in order.OrderItems)
            {
                order.TotalPrice += item.Product.Price * item.Quantity * item.Product.Discount.DiscountValue;
            }
        }
    }
}
