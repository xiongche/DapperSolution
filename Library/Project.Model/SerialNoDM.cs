using Project.Core;
using Project.Framework;
using System;

namespace Project.Model
{
    [Serializable]
    public class SerialNoDM : DbModel, IDbEntity
    {
        private static DbEntity dbEntity = new DbEntity()
        {
            Name = "C_SerialNo",
            IsNewSequentialPrimaryKey = true,
            PrimaryKeys = "SerialNoId"
        };

        public Guid SerialNoId
        { get; set; }

        public int Type
        { get; set; }

        public int Mode
        { get; set; }

        public string Prefix
        { get; set; }

        public int Length
        { get; set; }

        public int? Year
        { get; set; }

        public int? Month
        { get; set; }

        public int Count
        { get; set; }

        private int GetLength()
        {
            if (Count.ToString().Length > this.Length)
            {
                return Count.ToString().Length;
            }

            return Length;
        }

        private string GetYear()
        {
            if (Year.HasValue)
            {
                return Year.Value.ToString().Substring(2, 2);
            }

            return string.Empty;
        }

        private string GetMonth()
        {
            return this.Month.Value.ToString("D2");
        }

        private string GetCount()
        {
            string format = string.Format("D{0}", GetLength());

            return Count.ToString(format);
        }

        public string GetNo()
        {
            switch ((SerialNoMode)Mode)
            {
                case SerialNoMode.Year:
                    return string.Format("{0}{1}{2}", Prefix, GetYear(), GetCount());
                case SerialNoMode.Month:
                    string month = this.Month.Value.ToString("D2");
                    return string.Format("{0}{1}{2}{3}", Prefix, GetYear(), GetMonth(), GetCount());
                default:
                    return string.Format("{0}{1}", Prefix, GetCount());
            }
        }

        public DbEntity GetDbEntity()
        {
            return dbEntity;
        }

        public void SetPrimaryKey(Guid key)
        {
            this.SerialNoId = key;
        }

        public Guid GetPrimaryKey()
        {
            return this.SerialNoId;
        }
    }
}
