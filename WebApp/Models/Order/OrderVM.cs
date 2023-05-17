using Common.DataTransferObjects.Order;

namespace WebApp.Models.Order
{
    public class OrderVM
    {
        public int Id { get; set; }
        public int CashierId { get; set; }
        public string CashierName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
        public IEnumerable<OrderItemDetail> OrderItemList { get; set; }
    }
}
