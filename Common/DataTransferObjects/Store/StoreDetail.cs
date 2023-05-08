using Common.DataTransferObjects._Base;

namespace Common.DataTransferObjects.Store
{
    public class StoreDetail : SaveDTOExtension
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TransactionBy { get; set; }

    }
}
