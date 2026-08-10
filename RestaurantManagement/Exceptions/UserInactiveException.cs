using System;

namespace RestaurantManagement.Exceptions
{
    public class UserInactiveException : Exception
    {
        public UserInactiveException()
            : base("User account is inactive.")
        {
        }
    }
}
