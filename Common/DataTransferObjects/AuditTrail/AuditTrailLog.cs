namespace Common.DataTransferObjects.AuditTrail
{
    public class AuditTrailLog
    {
        public long AuditTrailId { get; set; }
        public string TransactionBy { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TotalAffectedTables { get; set; }
        public int TotalDataChanges { get; set; }
    }
}
