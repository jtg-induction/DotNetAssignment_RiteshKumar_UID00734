using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Restaurant
    {
        public long RestaurantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        public long OwnerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual User Owner { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
