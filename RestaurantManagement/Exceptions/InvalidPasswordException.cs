using System;

namespace RestaurantManagement.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
            : base("Current password is incorrect.")
        {
        }
    }
}
