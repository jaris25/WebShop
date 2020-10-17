using WebShop.Data.Entities;

namespace WebShop.Data.Repository
{
    public interface IOrdersRepository
    {
        void ConfirmOrder(Order order);
        bool ItemIsInStock(OrderItem item);
        void AddItemToOrder(OrderItem orderItem);
    }
}
