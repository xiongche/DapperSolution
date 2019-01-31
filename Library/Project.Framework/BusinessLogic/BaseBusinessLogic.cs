using System;
using System.Collections.Generic;
using System.Configuration;

namespace Project.Framework
{
    public abstract class BaseBusinessLogic : IShare, IDisposable
    {
        private IProxy proxy;

        protected IDbContext DB
        {
            get
            {
                return this.proxy.GetContext();
            }
        }

        protected IProxy Proxy
        {
            get
            {
                return this.proxy;
            }
        }

        public BaseBusinessLogic() : this(ConfigHelper.GetDbInstance()) { }

        protected BaseBusinessLogic(string instance)
        {
            this.proxy = new Proxy(instance);
        }

        public void SetProxy(IProxy proxy)
        {
            this.proxy = proxy;
        }

        public virtual void BulkInsert<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            DB.BulkInsert<T>(items, actedBy);
        }

        public virtual void BulkUpdate<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            DB.BulkUpdate<T>(items, actedBy);
        }

        public virtual void BulkDelete<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            DB.BulkDelete<T>(items, actedBy);
        }

        protected T Let<T>() where T : class, IShare, new()
        {
            return this.proxy.Get<T>();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }
            if (disposing && this.proxy != null)
            {
                this.proxy.Dispose();
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