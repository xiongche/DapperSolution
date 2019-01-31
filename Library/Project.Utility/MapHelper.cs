using Nelibur.ObjectMapper;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace System
{
    public sealed class MapHelper
    {
        public static TTo Map<TFrom, TTo>(TFrom from) where TTo : class
        {
            return from == null ? null : TinyMapper.Map<TTo>(from);
        }

        public static TTo Map<TTo>(DataRow row)
        {
            return DataRowEntityBuilder.GetBuilder<TTo>(row)(row);
        }

        public static IList<TTo> Map<TTo>(DataTable table)
        {
            var list = new List<TTo>();
            if (table.Rows.Count == 0)
            {
                return list;
            }

            var builder = DataRowEntityBuilder.GetBuilder<TTo>(table.Rows[0]);
            foreach (DataRow row in table.Rows)
            {
                list.Add(builder(row));
            }

            return list;
        }
    }

    public sealed class DataRowEntityBuilder
    {
        private static ConcurrentDictionary<string, Delegate> cache = new ConcurrentDictionary<string, Delegate>();

        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });

        public static Func<DataRow, T> GetBuilder<T>(DataRow row)
        {
            var key = $"{typeof(T)}DataRow{row.ItemArray.Length}";
            if (!cache.ContainsKey(key))
            {
                cache[key] = CreateBuilder<T>(row);
            }

            return cache[key] as Func<DataRow, T>;
        }

        private static Func<DataRow, T> CreateBuilder<T>(DataRow row)
        {
            var method = new DynamicMethod("DynamicCreate", typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
            var il = method.GetILGenerator();

            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                var property = typeof(T).GetProperty(row.Table.Columns[i].ColumnName);
                if (property == null || property.GetSetMethod() == null)
                {
                    continue;
                }

                Label endIfLabel = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Callvirt, isDBNullMethod);
                il.Emit(OpCodes.Brtrue, endIfLabel);

                il.Emit(OpCodes.Ldloc, result);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Callvirt, getValueMethod);
                il.Emit(OpCodes.Call, typeof(DataRowConvert).GetMethod($"To{GetTypeName(property)}", new Type[] { typeof(object) }));
                il.Emit(OpCodes.Callvirt, property.GetSetMethod());

                il.MarkLabel(endIfLabel);
            }

            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);

            return (Func<DataRow, T>)method.CreateDelegate(typeof(Func<DataRow, T>));
        }

        private static string GetTypeName(PropertyInfo property)
        {
            return IsNullable(property.PropertyType) ? $"Nullable{Nullable.GetUnderlyingType(property.PropertyType).Name}" : property.PropertyType.Name;
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    public sealed class DataRowConvert
    {
        public static Guid ToGuid(object value)
        {
            return value == null ? Guid.Empty : new Guid(value.ToString());
        }

        public static Boolean ToBoolean(object value)
        {
            return value == null ? false : Convert.ToBoolean(value);
        }

        public static Int16 ToInt16(object value)
        {
            return Convert.ToInt16(value);
        }

        public static Int32 ToInt32(object value)
        {
            return Convert.ToInt32(value);
        }

        public static Int64 ToInt64(object value)
        {
            return Convert.ToInt64(value);
        }

        public static Decimal ToDecimal(object value)
        {
            return Convert.ToDecimal(value);
        }

        public static String ToString(object value)
        {
            return value == null ? null : value.ToString();
        }

        public static DateTime ToDateTime(object value)
        {
            return Convert.ToDateTime(value);
        }

        public static Guid? ToNullableGuid(object value)
        {
            if (value == null)
            {
                return null;
            }

            return new Guid(value.ToString());
        }

        public static Boolean? ToNullableBoolean(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToBoolean(value);
        }

        public static Int16? ToNullableInt16(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt16(value);
        }

        public static Int32? ToNullableInt32(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt32(value);
        }

        public static Int64? ToNullableInt64(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt64(value);
        }

        public static Decimal? ToNullableDecimal(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToDecimal(value);
        }

        public static DateTime? ToNullableDateTime(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToDateTime(value);
        }
    }
}
