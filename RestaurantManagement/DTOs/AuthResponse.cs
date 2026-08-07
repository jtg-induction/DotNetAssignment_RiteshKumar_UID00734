using System;

namespace RestaurantManagement.DTOs.Responses
{
    public class AuthResponse
    {
        public long UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
