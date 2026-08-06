using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class User
    {
        public long UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        [EmailAddress]
        [Index("IX_User_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [Index("IX_User_Phone", IsUnique = true)]
        public string Phone { get; set; }

        public int RoleId { get; set; }

        [Column(TypeName = "decimal")]
        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; } = 1000;

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual Role Role { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
