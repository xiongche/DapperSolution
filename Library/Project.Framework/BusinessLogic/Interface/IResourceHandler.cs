using System;

namespace Project.Framework
{
    public interface IResourceHandler : IDisposable
    {
        void SetFunc(string key, Delegate func);

        Delegate GetFunc(string key);
    }
}
