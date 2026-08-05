using RestaurantManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Order
    {
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public long RestaurantId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public OrderStatus Status { get; set; }

        [Column(TypeName = "decimal")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public virtual User User { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }

}