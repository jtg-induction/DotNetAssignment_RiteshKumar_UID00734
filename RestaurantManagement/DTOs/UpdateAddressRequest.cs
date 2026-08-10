using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.DTOs.Requests
{
    public class UpdateAddressRequest
    {
        [StringLength(100, MinimumLength = 1)]
        public string RecipientName { get; set; }

        [StringLength(10, MinimumLength = 10)]
        public string Phone { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string AddressLine1 { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string AddressLine2 { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string City { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string State { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string PostalCode { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string Country { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string Landmark { get; set; }
    }
}
