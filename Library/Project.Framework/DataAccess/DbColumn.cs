namespace Project.Framework
{
    internal sealed class DbColumn
    {
        public const string SQL_COLUMN_DEFINITION = "SELECT COLUMN_NAME AS Name,UPPER(DATA_TYPE) AS DataType,CHARACTER_MAXIMUM_LENGTH AS CharacterMaximumLength,NUMERIC_PRECISION AS NumericPrecision,NUMERIC_SCALE AS NumericScale FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TableName ORDER BY ORDINAL_POSITION";

        public string Name { get; set; }

        public string DataType { get; set; }

        public string CharacterMaximumLength { get; set; }

        public int? NumericPrecision { get; set; }

        public int? NumericScale { get; set; }

        public string GetColumnDefinition()
        {
            switch (this.DataType)
            {
                case "VARCHAR":
                case "NVARCHAR":
                case "CHAR":
                    var length = this.CharacterMaximumLength == "-1" ? "MAX" : this.CharacterMaximumLength;
                    return string.Format("[{0}] {1}({2})", this.Name, this.DataType, length);
                case "DECIMAL":
                    return string.Format("[{0}] DECIMAL({1},{2})", this.Name, this.NumericPrecision, this.NumericScale);
                default:
                    return string.Format("[{0}] {1}", this.Name, this.DataType);
            }
        }
    }
}
