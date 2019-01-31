using System;

namespace Project.Framework
{
    public interface IDbEntity : IDbQuery
    {
        void SetPrimaryKey(Guid key);

        Guid GetPrimaryKey();
    }
}
