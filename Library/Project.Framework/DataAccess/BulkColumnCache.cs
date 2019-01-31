using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Project.Framework
{
    internal sealed class BulkColumnCache<T> where T : class, new()
    {
        private static ConcurrentDictionary<Type, IDictionary<string, Type>> columnCache = new ConcurrentDictionary<Type, IDictionary<string, Type>>();

        public static IDictionary<string, Type> GetColumns()
        {
            var type = typeof(T);
            if (!columnCache.Keys.Contains(type))
            {
                columnCache[type] = GenerateColumns();
            }

            return columnCache[type];
        }

        private static IDictionary<string, Type> GenerateColumns()
        {
            var columns = new Dictionary<string, Type>();

            T item = new T();
            foreach (var helper in PropertyHelper.GetProperties(item))
            {
                columns[helper.Name] = helper.Type.IsGenericType ? Nullable.GetUnderlyingType(helper.Type) : helper.Type;
            }

            columns["IsActive"] = typeof(bool);
            //columns.Remove("Timestamp");
            return columns;
        }
    }
}
