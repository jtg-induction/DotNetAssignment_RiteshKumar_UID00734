using System;

namespace RestaurantManagement.Exceptions
{
    public class InactiveUserException : Exception
    {
        public InactiveUserException()
            : base("User account is inactive.")
        {
        }
    }
}
