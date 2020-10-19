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
                    var supplier = GetAvailableSupplier(item);
                    // Additional check to see if item is still available when confirming order.
                    if (supplier == null)
                    {
                        order.TotalPrice -= item.Product.Price * item.Quantity;
                        _context.OrderItems.Remove(item);
                        continue;
                    }
                    supplier.Quantity -= item.Quantity;
                }
                _context.Orders.Update(order);
                _context.SaveChanges();
            }
        }

        public void AddItemToOrder(OrderItem orderItem, Order order)
        {
            if (GetAvailableSupplier(orderItem) != null)
            {
                order.OrderItems.Add(orderItem);
            }
            _context.Orders.Add(order);
        }

        //TODO: Create method to order the amount of items that are still in stock if amount exceeds suppliers stock
        public Supplier GetAvailableSupplier(OrderItem item)
        {
            return item.Product.ProductSuppliers.Where(ps => ps.Supplier.Quantity >= item.Quantity).Select(ps => ps.Supplier).FirstOrDefault();
        }
    }
}
