namespace Common.DataTransferObjects.AuditTrail
{
    public class AuditTrailLogDetail : AuditTrailLog
    {
        public IEnumerable<AuditTrailChange> AuditTrailChanges { get; set; }
    }

    public class AuditTrailChange
    {
        public string PrimaryKey { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
