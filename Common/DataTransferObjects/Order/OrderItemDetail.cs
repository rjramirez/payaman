using Common.DataTransferObjects._Base;

namespace Common.DataTransferObjects.Order
{
    public class OrderItemDetail : SaveDTOExtension
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }
        public bool Active { get; set; }
    }
}
