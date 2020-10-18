using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Data.Entities
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<ProductSupplier> ProductSuppliers { get; set; }
    }
}
