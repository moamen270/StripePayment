namespace StripePayment.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime CreationDate { get; set; }
		public int Quantity { get; set; }
		public int ProductId { get; set; }
		public string Status { get; set; }
		public double TotalPrice { get; set; }
		public Product Product { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
		public string? SessionId { get; set; }
	}
}