using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class RefreshToken
    {
        public long RefreshTokenId { get; set; }

        public long UserId { get; set; }

        [Required]
        [StringLength(256)]
        [Index("IX_RefreshToken_TokenHash", IsUnique = true)]
        public string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public virtual User User { get; set; }
    }
}