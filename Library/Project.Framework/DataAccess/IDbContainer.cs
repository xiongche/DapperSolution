using System;

namespace Project.Framework
{
    public interface IDbContainer : IDisposable
    {
        IDbContext GetContext(string instance);

        void SetContext(string instance);

        void BeginTran();

        void Commit();

        void Rollback();
    }
}
