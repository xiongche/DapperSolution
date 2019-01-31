using System;
using System.Collections.Generic;

namespace Project.Framework
{
    public sealed class DbEntityCache
    {
        private static IDictionary<Type, DbEntity> cache = new Dictionary<Type, DbEntity>();

        public static DbEntity GetDbEntity(Type type)
        {
            if (!cache.ContainsKey(type))
            {
                cache[type] = InnerGetDbEntity(Activator.CreateInstance(type));
            }

            return cache[type];
        }

        public static DbEntity GetDbEntity<T>() where T : new()
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                cache[typeof(T)] = InnerGetDbEntity(new T());
            }

            return cache[typeof(T)];
        }

        public static DbEntity GetDbEntity<T>(T t) where T : class, IDbQuery, new()
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                cache[typeof(T)] = t.GetDbEntity();
            }

            return cache[typeof(T)];
        }

        public static string GetEntityName(Type type)
        {
            return GetDbEntity(type).FullName;
        }

        public static string GetEntityName<T>() where T : class, IDbQuery, new()
        {
            return GetDbEntity<T>().FullName;
        }

        private static DbEntity InnerGetDbEntity(object value)
        {
            var dbEntity = value as IDbQuery;
            if (dbEntity == null)
            {
                throw new InvalidOperationException("Not inherit IDbQuery");
            }

            return dbEntity.GetDbEntity();
        }
    }
}
