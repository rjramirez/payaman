namespace Common.DataTransferObjects.AuditTrail
{
    public class ContextChangeTrackingDetail
    {
        public string EntityId { get; set; }
        public string TableName { get; set; }
        public string EntityField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Action { get; set; }
    }
}
