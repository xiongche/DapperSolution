using System;

namespace Project.Framework
{
    public interface IDbModel
    {
        Guid CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        Guid? UpdatedBy { get; set; }

        DateTime? UpdatedDate { get; set; }

        string HashValue { get; set; }

        //byte[] Timestamp { get; set; }
    }
}
