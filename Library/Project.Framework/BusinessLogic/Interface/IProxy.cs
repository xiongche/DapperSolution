using System;

namespace Project.Framework
{
    public interface IProxy : IDisposable
    {
        IDbContext GetContext();

        T Get<T>() where T : class, IShare, new();

        object Get(Type type);
    }
}
