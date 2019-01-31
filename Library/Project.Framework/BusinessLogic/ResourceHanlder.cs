using System;
using System.Collections.Generic;

namespace Project.Framework
{
    public sealed class ResourceHandler : IResourceHandler
    {       
        private IDictionary<string, Delegate> functions;

        public ResourceHandler()
        {
            this.functions = new Dictionary<string, Delegate>();
        }

        public Delegate GetFunc(string key)
        {
            return functions[key];
        }

        public void SetFunc(string key, Delegate func)
        {
            functions.Add(key, func);
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
                this.functions.Clear();
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
