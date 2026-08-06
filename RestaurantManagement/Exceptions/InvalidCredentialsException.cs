using System;

namespace RestaurantManagement.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.")
        {
        }
    }
}
