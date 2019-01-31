using System;

namespace Project.Framework
{
    public abstract class BasePresentationService : IDisposable
    {
        private IProxy proxy;
        private IResourceHandler handler;

        public IResourceHandler Handler
        {
            get
            {
                if (this.handler == null)
                {
                    this.handler = new ResourceHandler();
                }

                return this.handler;
            }
        }

        protected BasePresentationService(string instance)
        {
            this.proxy = new Proxy(instance);
        }

        public void BeginTran()
        {
            this.proxy.GetContext().BeginTran();
        }

        public void Commit()
        {
            try
            {
                this.proxy.GetContext().Commit();
            }
            catch
            {
                this.proxy.GetContext().Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            this.proxy.GetContext().Rollback();
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
