using Common.DataTransferObjects._Base;
using Common.DataTransferObjects.Order;

namespace Common.DataTransferObjects.Cart
{
    public class CartDetail : SaveDTOExtension
    {
        public int CashierId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
