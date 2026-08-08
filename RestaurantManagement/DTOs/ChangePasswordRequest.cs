using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }


        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }


        [Required]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
