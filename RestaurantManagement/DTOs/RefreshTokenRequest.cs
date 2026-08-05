using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}