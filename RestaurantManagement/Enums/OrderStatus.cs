namespace RestaurantManagement.Enums
{
    public enum OrderStatus : byte
    {
        Placed = 1,
        Accepted = 2,
        Rejected = 3,
        Dispatched = 4,
        Delivered = 5,
        Cancelled = 6
    }
}