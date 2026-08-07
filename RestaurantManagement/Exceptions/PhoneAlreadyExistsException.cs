using System;

namespace RestaurantManagement.Exceptions
{
    public class PhoneAlreadyExistsException : Exception
    {
        public PhoneAlreadyExistsException(string phone)
            : base($"Phone number '{phone}' is already registered.")
        {
        }
    }
}
