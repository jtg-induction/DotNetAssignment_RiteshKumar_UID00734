using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(72)]
        public string Password { get; set; }

    }
}
