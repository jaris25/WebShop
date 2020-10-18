using WebShop.Data.Entities;

namespace WebShop.Data.Repository
{
    public interface IOrdersRepository
    {
        void ConfirmOrder(Order order);
        Supplier GetAvailableSupplier(OrderItem item);
        void AddItemToOrder(OrderItem orderItem, Order order);
    }
}
