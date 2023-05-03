using Common.DataTransferObjects._Base;

namespace Common.DataTransferObjects.Cart
{
    public class CartDetail : SaveDTOExtension
    {
        public int ProductId { get; set; }
        public int CashierId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
    }
}
