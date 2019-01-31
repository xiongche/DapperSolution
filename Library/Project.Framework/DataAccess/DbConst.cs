namespace Project.Framework
{
    internal sealed class DbConst
    {
        public static readonly string[] AuditProperties = new string[] { "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate", "HashValue" };

        public static readonly string[] InsertProperties = new string[] { "CreatedBy", "CreatedDate" };
        public static readonly string[] UpdateProperties = new string[] { "UpdatedBy", "UpdatedDate" };

        public static readonly string[] BulkUpdateExcludedColumns = new string[] { "CreatedBy", "CreatedDate", "Timestamp" };
        //public static readonly string[] BulkInsertExcludedColumns = new string[] { "UpdatedBy", "UpdatedDate", "Timestamp" };
    }
}
