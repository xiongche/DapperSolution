using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System
{
    public sealed class TypeHelper
    {
        public static IDictionary<string, string> ToPairs<T>(T t)
        {
            if (t == null)
            {
                return new Dictionary<string, string>();
            }

            var builder = StringPairsBuilder.GetBuilder<T>();
            return builder(t);
        }
    }

    public sealed class StringPairsBuilder
    {
        private static ConcurrentDictionary<string, Delegate> cache = new ConcurrentDictionary<string, Delegate>();
        private static readonly MethodInfo setValueMethod = typeof(Dictionary<string, string>).GetMethod("set_Item", new Type[] { typeof(string), typeof(string) });

        public static Func<T, Dictionary<string, string>> GetBuilder<T>()
        {
            var key = $"{typeof(T)}";
            if (!cache.ContainsKey(key))
            {
                cache[key] = CreateBuilder<T>();
            }

            return cache[key] as Func<T, Dictionary<string, string>>;
        }

        private static Func<T, Dictionary<string, string>> CreateBuilder<T>()
        {
            var method = new DynamicMethod("StringPairs", typeof(Dictionary<string, string>), new Type[] { typeof(T) });

            var il = method.GetILGenerator();

            LocalBuilder result = il.DeclareLocal(typeof(Dictionary<string, string>));
            il.Emit(OpCodes.Newobj, typeof(Dictionary<string, string>).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            var properties = GetSupportProperies(typeof(T));
            foreach (var propery in properties)
            {
                il.Emit(OpCodes.Ldloc, result);
                il.Emit(OpCodes.Ldstr, propery.Name);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, propery.GetMethod);
                il.Emit(OpCodes.Call, typeof(StringConvert).GetMethod($"ToString", new Type[] { propery.PropertyType }));
                il.Emit(OpCodes.Callvirt, setValueMethod);
            }

            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);

            return (Func<T, Dictionary<string, string>>)method.CreateDelegate(typeof(Func<T, Dictionary<string, string>>));
        }

        private static bool IsSupportPropery(PropertyInfo property)
        {
            return property.MemberType == MemberTypes.Property && property.CanRead;
        }

        private static IEnumerable<PropertyInfo> GetSupportProperies(Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            return type.GetProperties(flags).Where(property => IsSupportPropery(property));
        }
    }

    public sealed class StringConvert
    {
        public static string ToString(Guid value)
        {
            return value.ToString();
        }

        public static string ToString(Int16? value)
        {
            return value.HasValue ? value.Value.ToString() : null;
        }

        public static string ToString(Int32? value)
        {
            return value.HasValue ? value.Value.ToString() : null;
        }

        public static string ToString(Int64? value)
        {
            return value.HasValue ? value.Value.ToString() : null;
        }

        public static string ToString(Int16 value)
        {
            return value.ToString();
        }

        public static string ToString(Int32 value)
        {
            return value.ToString();
        }

        public static string ToString(Int64 value)
        {
            return value.ToString();
        }

        public static string ToString(string value)
        {
            return value;
        }
    }
}
