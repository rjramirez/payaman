namespace WebApp.Models.Cart
{
    public class CartVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int CashierId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
    }
}
