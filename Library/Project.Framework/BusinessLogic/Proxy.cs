using System;
using System.Collections.Concurrent;

namespace Project.Framework
{
    public sealed class Proxy : IProxy
    {
        private IDbContext context;

        private ConcurrentDictionary<Type, object> cache;

        public Proxy(string instance) : this(new DbContext(instance)) { }

        public Proxy(IDbContext context)
        {
            this.cache = new ConcurrentDictionary<Type, object>();
            this.context = context;
        }

        public T Get<T>() where T : class, IShare, new()
        {
            if (!cache.Keys.Contains(typeof(T)))
            {
                T t = new T();
                t.SetProxy(this);

                cache[typeof(T)] = t;
            }

            return cache[typeof(T)] as T;
        }

        public object Get(Type type)
        {
            if (!cache.Keys.Contains(type))
            {
                var t = Activator.CreateInstance(type) as IShare;
                t.SetProxy(this);

                cache[type] = t;
            }

            return cache[type];
        }

        public IDbContext GetContext()
        {
            return this.context;
        }

        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                cache.Clear();
            }
            if (this.context != null)
            {
                this.context.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}