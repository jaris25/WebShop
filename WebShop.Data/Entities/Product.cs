using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Data.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string BrandName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        [ForeignKey("DiscountId")]
        public Discount Discount { get; set; }
        public int? DiscountId { get; set; }
 
    }
}
