using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class UserAddress
    {
        public long UserAddressId { get; set; }

        public long UserId { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string AddressLine1 { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string AddressLine2 { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string City { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string State { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Country { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string Landmark { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; }
    }
}