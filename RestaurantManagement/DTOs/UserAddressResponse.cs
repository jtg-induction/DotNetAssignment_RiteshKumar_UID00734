namespace RestaurantManagement.DTOs.Responses
{
    public class UserAddressResponse
    {
        public long UserAddressId { get; set; }

        public string RecipientName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Landmark { get; set; }
    }
}
