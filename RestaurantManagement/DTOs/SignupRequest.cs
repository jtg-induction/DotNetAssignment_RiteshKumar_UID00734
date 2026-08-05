using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class SignupRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }
    }
}