namespace Common.DataTransferObjects.Order
{
    public class OrderSearchResult
    {
        public int Id { get; set; }
        public int CashierId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public IEnumerable<OrderItemDetail> OrderItemList { get; set; }
    }
}
