using Common.DataTransferObjects.Order;

namespace WebApp.Models.Cart
{
    public class CartVM
    {
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public int OrderId { get; set; }
        public string CashierName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
        public IEnumerable<OrderItemDetail> OrderItemList { get; set; }
    }
}
