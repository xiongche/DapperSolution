using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Project.Framework
{
    public class DbContainer : IDbContainer
    {
        private bool isInTran = false;
        private ConcurrentDictionary<string, IDbContext> dbContextCache = new ConcurrentDictionary<string, IDbContext>();

        public IDbContext GetContext(string instance)
        {
            if (!this.dbContextCache.ContainsKey(instance))
            {
                SetContext(instance);
            }

            return dbContextCache[instance] as IDbContext;
        }

        public void SetContext(string instance)
        {
            if (this.dbContextCache.ContainsKey(instance))
            {
                return;
            }

            var dbContext = new DbContext(instance);
            if (isInTran)
            {
                dbContext.BeginTran();
            }

            this.dbContextCache[instance] = dbContext;
        }

        public DbContainer() { }

        public DbContainer(string instance)
        {
            SetContext(instance);
        }

        public void BeginTran()
        {
            this.isInTran = true;
        }

        public void Commit()
        {
            dbContextCache.Keys.Each(key => ((IDbContext)dbContextCache[key]).Commit());
        }

        public void Rollback()
        {
            dbContextCache.Keys.Each(key => ((IDbContext)dbContextCache[key]).Rollback());
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
                dbContextCache.Keys.Each(key => ((IDisposable)dbContextCache[key]).Dispose());
                dbContextCache.Clear();
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