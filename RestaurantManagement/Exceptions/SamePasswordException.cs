using System;

namespace RestaurantManagement.Exceptions
{
    public class SamePasswordException : Exception
    {
        public SamePasswordException()
            : base("New password cannot be the same as current password.")
        {
        }
    }
}
