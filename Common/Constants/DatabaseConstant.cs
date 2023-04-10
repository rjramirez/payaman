namespace Common.Constants
{
    public static class DbContextConstant
    {
        //TODO: Columns that don't need audit trail
        public const string NoAuditColumns = "CreatedBy,CreatedDate,UpdatedBy,UpdatedDate";
    }

    public static class StoredProcedureConstant
    {
        public const string ErrorLogInsertSpName = "usp_errorloginsert";
        public const string SpParamName_Message = "@message";
        public const string SpParamName_StackTrace = "@stackTrace";
        public const string SpParamName_Path = "@path";
        public const string SpParamName_StackTraceId = "@stackTraceId";
        public const string SpParamName_Source = "@source";
        public const string SpParamName_UserIdentity = "@userIdentity";
        public const string SpParamName_BuildVerion = "@buildVersion";
    }
}
