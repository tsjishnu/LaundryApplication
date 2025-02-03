namespace LaundryApplication.Models
{
    public enum OrderStatus
    {
        Pending,
        InProgress,
        Completed,
        Delivered
    }

    public class Order
    {
        public Guid Id { get; set; }  // Unique ID for the order
        public Guid ServiceId { get; set; }  // The ID of the selected service
        public Service Service { get; set; }  // Navigation property to the Service
        public int Quantity { get; set; }  // The number of items for the service
        public DateTime ExpectedDeliveryDate { get; set; }  // Expected delivery date
        public string AdditionalDescription { get; set; }  // Optional additional description from the customer
        public OrderStatus Status { get; set; }  // Current status of the order
        public DateTime DateCreated { get; set; }  // Timestamp for when the order was created
    }
}
