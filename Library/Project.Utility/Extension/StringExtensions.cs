using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace System.Text
{
    public static class StringExtension
    {
        public static IEnumerable<int> AsInts(this string source)
        {
            return AsInts(source, ',');
        }

        public static IEnumerable<int> AsInts(this string source, char separator)
        {
            return source.Split(separator).Select(s => ConvertToInt(s));
        }

        public static IEnumerable<Guid> AsGuids(this string source)
        {
            return AsGuids(source, ',');
        }

        public static IEnumerable<Guid> AsGuids(this string source, char separator)
        {
            return source.Split(separator).Select(s => ConvertToGuid(s));
        }

        private static Guid ConvertToGuid(string value)
        {
            var result = Guid.Empty;
            Guid.TryParse(value, out result);

            return result;
        }

        public static int ConvertToInt(string value)
        {
            int result = default(int);
            int.TryParse(value, out result);

            return result;
        }
    }
}
