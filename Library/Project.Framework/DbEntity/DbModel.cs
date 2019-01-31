using System;

namespace Project.Framework
{
    [Serializable]
    public class DbModel : IDbModel
    {
        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        //public byte[] Timestamp { get; set; }

        public string HashValue { get; set; }
    }
}