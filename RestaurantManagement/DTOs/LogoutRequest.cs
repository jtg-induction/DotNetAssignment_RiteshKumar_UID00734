using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestaurantManagement.DTOs.Requests
{
    public class LogoutRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
