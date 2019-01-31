using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System
{
    public sealed class EnumHelper
    {
        public static int GetValueByDescription(IDictionary<string, int> pais, string description, int defaultValue)
        {
            if (pais.Count == 0 && string.IsNullOrEmpty(description))
            {
                return defaultValue;
            }

            var key = pais.Keys.FirstOrDefault(model => StringHelper.Compare(model, description));
            return string.IsNullOrEmpty(key) ? defaultValue : pais[key];
        }

        public static string GetDescription<T>(T value) where T : struct, IConvertible
        {
            ThrowIfNotEnum<T>();

            var type = value.GetType();
            var field = type.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length == 0)
            {
                return Enum.GetName(type, value);
            }

            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public static string GetDescription<T>(int value) where T : struct, IConvertible
        {
            return GetDescription<T>(ToEnum<T>(value));
        }

        public static string GetDescription<T>(object value) where T : struct, IConvertible
        {
            var enumValue = ConvertHelper.ToInt(value);
            return enumValue < 0 ? string.Empty : GetDescription<T>(ToEnum<T>(enumValue));
        }

        public static EnumModel GetEnum<T>(int value) where T : struct, IConvertible
        {
            return new EnumModel(GetDescription<T>(value), value);
        }

        public static IEnumerable<int> GetValues<T>() where T : struct, IConvertible
        {
            var pairs = GetNamePairs<T>();

            return pairs.Keys.Select(key => pairs[key]);
        }

        public static IEnumerable<int> GetValues<T>(string keys) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(keys))
            {
                return new List<int>();
            }

            var pairs = GetNamePairs<T>();
            var upperKeys = StringHelper.ToPureUpper(keys);

            return upperKeys.Split(',').Where(key => pairs.ContainsKey(key)).Select(key => pairs[key]);
        }

        public static IEnumerable<string> GetNames<T>() where T : struct, IConvertible
        {
            return GetNamePairs<T>().Keys;
        }

        public static IEnumerable<string> GetDescriptions<T>() where T : struct, IConvertible
        {
            return GetDescriptionPairs<T>().Keys;
        }

        public static IDictionary<string, int> GetDescriptionPairs<T>() where T : struct, IConvertible
        {
            ThrowIfNotEnum<T>();

            var pairs = new Dictionary<string, int>();

            var values = (int[])Enum.GetValues(typeof(T));
            values.Each(value => pairs[StringHelper.ToPureUpper(GetDescription<T>(value))] = value);

            return pairs;
        }

        public static IDictionary<string, int> GetNamePairs<T>() where T : struct, IConvertible
        {
            ThrowIfNotEnum<T>();
            return GetNamePairs(typeof(T));
        }

        public static IDictionary<string, int> GetNamePairs(Type type)
        {
            var pairs = new Dictionary<string, int>();

            var values = (int[])Enum.GetValues(type);
            values.Each(value => pairs[StringHelper.ToPureUpper(Enum.Parse(type, value.ToString()))] = value);

            return pairs;
        }

        public static IEnumerable<EnumModel> GetEnums<T>() where T : struct, IConvertible
        {
            var pairs = GetDescriptionPairs<T>();
            return pairs.Keys.Select(key => new EnumModel(key, pairs[key]));
        }

        private static void ThrowIfNotEnum<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }

        private static T ToEnum<T>(int value) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }

    [Serializable]
    public class EnumModel
    {
        public string Key
        { get; set; }

        public int Value
        { get; set; }

        public EnumModel() { }

        public EnumModel(string key, int value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
