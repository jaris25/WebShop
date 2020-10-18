using System.Linq;
using WebShop.Data.Contexts;
using WebShop.Data.Entities;

namespace WebShop.Data.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly WebShopContext _context;
        private readonly object supplyLock = new object();

        public OrdersRepository(WebShopContext context)
        {
            _context = context;
        }

        public void ConfirmOrder(Order order)
        {
            lock (supplyLock)
            {
                foreach (var item in order.OrderItems)
                {
                    // Additional check to see if item is still available when confirming order.
                    if (!ItemIsInStock(item))
                    {
                        order.TotalPrice -= item.Product.Price * item.Quantity;
                        continue;
                    }
                    var supplier = item.Product.Suppliers.Where(s => s.Quantity >= item.Quantity).FirstOrDefault();
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

        //TODO: Create method to order the amount of items that are still in stock
        public bool ItemIsInStock(OrderItem item)
        {
            return item.Product.Suppliers.Where(s => s.Quantity >= item.Quantity).FirstOrDefault() != null ? true : false;
        }

    }
}
