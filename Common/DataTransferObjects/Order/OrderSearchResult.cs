namespace Common.DataTransferObjects.Order
{
    public class OrderSearchResult
    {
        //public int Id { get; set; }
        //public int CashierId { get; set; }
        //public string CashierName { get; set; }
        //public decimal TotalAmount { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public IEnumerable<OrderItemDetail> OrderItemList { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
    }
}
