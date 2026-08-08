using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class PatchProfileRequest
    {
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(10, MinimumLength = 10)]
        public string Phone { get; set; }
    }
}
