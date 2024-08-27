namespace Ecommerce.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
