using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }

        public long OrderId { get; set; }

        public long MenuItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }

        public virtual MenuItem MenuItem { get; set; }
    }
}
