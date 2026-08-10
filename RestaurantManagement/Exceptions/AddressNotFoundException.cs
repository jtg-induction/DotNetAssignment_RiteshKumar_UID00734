using System;

namespace RestaurantManagement.Exceptions
{
    public class AddressNotFoundException : Exception
    {
        public AddressNotFoundException()
            : base("Address not found.")
        {
        }
    }
}
