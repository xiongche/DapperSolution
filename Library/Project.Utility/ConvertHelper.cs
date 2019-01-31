using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    public sealed class ConvertHelper
    {
        private static IEnumerable<string> alphaList = new List<string>()
        {
            string.Empty, "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        #region Convert To DateTime

        public static DateTime ToDate(string value, string format, IFormatProvider formatProvider)
        {
            value = value.Trim();

            DateTime dateTime = default(DateTime);
            DateTime.TryParseExact(value, format, formatProvider, DateTimeStyles.None, out dateTime);

            return dateTime;
        }

        public static DateTime ToDate(object value, string format, IFormatProvider formatProvider)
        {
            if (value != null)
            {
                return ToDate(value.ToString(), format, formatProvider);
            }
            else
            {
                return default(DateTime);
            }
        }

        public static DateTime ToDate(string value, string format)
        {
            return ToDate(value, format, CultureInfo.CurrentCulture);
        }

        public static DateTime ToDate(object value, string format)
        {
            if (value != null)
            {
                return ToDate(value.ToString(), format);
            }
            else
            {
                return default(DateTime);
            }
        }

        public static DateTime ToDate(string value)
        {
            value = value.Trim();

            DateTime dateTime = default(DateTime);
            DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);

            return dateTime;
        }

        public static DateTime ToDate(object value)
        {
            if (value != null)
            {
                try
                {
                    return Convert.ToDateTime(value);
                }
                catch
                {
                    return ToDate(value.ToString());
                }
            }
            else
            {
                return default(DateTime);
            }
        }

        public static DateTime? ToNullableDate(string value, string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return ToDate(value, format, formatProvider);
        }

        public static DateTime? ToNullableDate(object value, string format, IFormatProvider formatProvider)
        {
            if (value == null)
            {
                return null;
            }

            return ToNullableDate(value.ToString(), format, formatProvider);
        }

        public static DateTime? ToNullableDate(string value, string format)
        {
            return ToNullableDate(value, format, CultureInfo.CurrentCulture);
        }

        public static DateTime? ToNullableDate(object value, string format)
        {
            return ToNullableDate(value, format, CultureInfo.CurrentCulture);
        }

        public static DateTime? ToNullableDate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return ToDate(value);
        }

        public static DateTime? ToNullableDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return ToNullableDate(value.ToString());
            }
        }

        #endregion

        #region Convert To Int

        public static int? ToNullableInt(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return ToInt(value);
        }

        public static int ToInt(string value)
        {
            int result = default(int);

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                int.TryParse(value, out result);
            }

            return result;
        }

        public static int ToInt(object value)
        {
            int result = default(int);

            if (value != null)
            {
                int.TryParse(value.ToString(), out result);
            }

            return result;
        }

        #endregion

        #region Convert To Decimal

        public static decimal ToDecimal(string value)
        {
            value = value.Trim();

            decimal result = default(decimal);
            decimal.TryParse(value, out result);

            return result;
        }

        public static decimal ToDecimal(object value)
        {
            decimal result = default(decimal);

            if (value != null)
            {
                decimal.TryParse(value.ToString(), out result);
            }

            return result;
        }

        public static decimal? ToNullableDecimal(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return ToDecimal(value);
        }

        #endregion

        #region Convert To Double

        //public static double ToDouble(string value)
        //{
        //    value = value.Trim();

        //    double result = default(double);
        //    double.TryParse(value, out result);

        //    return result;
        //}

        //public static double ToDouble(object value)
        //{
        //    double result = default(double);

        //    if (value != null)
        //    {
        //        double.TryParse(value.ToString(), out result);
        //    }

        //    return result;
        //}

        //public static double? ToNullableDouble(object value)
        //{
        //    if (value == null || string.IsNullOrEmpty(value.ToString()))
        //    {
        //        return null;
        //    }

        //    return ToDouble(value);
        //}

        #endregion

        #region Convert To Bool

        public static bool? ToNullableBool(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return ToBool(value);
        }

        public static bool ToBool(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();

                if (value == "1")
                {
                    return true;
                }

                if (value == "0")
                {
                    return false;
                }
            }

            bool result = false;
            bool.TryParse(value, out result);

            return result;
        }

        public static bool ToBool(object value)
        {
            return value != null ? ToBool(value.ToString()) : false;
        }

        #endregion

        #region Convert To String

        public static string ToString(DateTime value, string format, IFormatProvider formatProvider)
        {
            return value.ToString(format, formatProvider);
        }

        public static string ToString(DateTime value, string format)
        {
            return value.ToString(format, CultureInfo.CurrentCulture);
        }

        public static string ToNullableString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return value.ToString();
        }

        public static string ToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static string ToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }

        public static string ToTitleCase(CultureInfo culture, string format)
        {
            TextInfo textInfo = culture.TextInfo;

            return textInfo.ToTitleCase(format);
        }

        public static string ToTitleCase(string format)
        {
            return ToTitleCase(CultureInfo.CurrentCulture, format);
        }

        public static string ToBase64String(string text)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(utf8Bytes);
        }

        #endregion

        #region Convert To Guid

        public static Guid? ToNullableGuid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) || value == DBNull.Value)
            {
                return null;
            }

            return ToGuid(value);
        }

        public static Guid ToGuid(string value)
        {
            try
            {
                return string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static Guid ToGuid(object value)
        {
            return value != null ? ToGuid(value.ToString()) : Guid.Empty;
        }

        #endregion

        #region Convert to Timpspan

        public static TimeSpan ToTimeSpan(string value)
        {
            return ToDate(value).TimeOfDay;
        }

        public static TimeSpan ToTimeSpan(object value)
        {
            if (value != null)
            {
                return ToTimeSpan(value.ToString());
            }

            return new TimeSpan();
        }

        #endregion

        #region Convert to SqlDateTime

        public static DateTime ToSqlDate(object value)
        {
            return ToSqlMinDate(value);
        }

        public static DateTime ToSqlMinDate(object value, string format, IFormatProvider formatProvider)
        {
            DateTime result = ToDate(value, format, formatProvider);

            if (result == default(DateTime))
            {
                return SqlDateTime.MinValue.Value;
            }

            return result;
        }

        public static DateTime ToSqlMinDate(object value, string format)
        {
            DateTime result = ToDate(value, format);

            if (result == default(DateTime))
            {
                return SqlDateTime.MinValue.Value;
            }

            return result;
        }

        public static DateTime ToSqlMinDate(object value)
        {
            DateTime result = ToDate(value);

            if (result == default(DateTime))
            {
                return SqlDateTime.MinValue.Value;
            }

            return result;
        }

        public static DateTime ToSqlMaxDate(object value, string format, IFormatProvider formatProvider)
        {
            DateTime result = ToDate(value, format, formatProvider);

            if (result == default(DateTime))
            {
                return SqlDateTime.MaxValue.Value;
            }

            return result;
        }

        public static DateTime ToSqlMaxDate(object value, string format)
        {
            DateTime result = ToDate(value, format);

            if (result == default(DateTime))
            {
                return SqlDateTime.MaxValue.Value;
            }

            return result;
        }

        public static DateTime ToSqlMaxDate(object value)
        {
            DateTime result = ToDate(value);

            if (result == default(DateTime))
            {
                return SqlDateTime.MaxValue.Value;
            }

            return result;
        }

        #endregion

        #region Convert to Time Format

        public static string ToFormattedTime(int seconds)
        {
            int hour = seconds / 3600;
            int minute = seconds % 3600 / 60;
            int second = seconds % 60;

            return string.Format("{0,2:00}:{1,2:00}:{2,2:00}", hour, minute, second);
        }

        #endregion

        public static string ToHtml(DataTable table)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<table style='border-collapse: collapse;' border=1 >");
            html.Append("<tr>");

            foreach (DataColumn column in table.Columns)
            {
                html.Append("<th>");
                html.Append(column.ColumnName);
                html.Append("</th>");
            }

            html.Append("</tr>");

            foreach (DataRow row in table.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn column in table.Columns)
                {
                    html.Append("<td>");
                    html.Append(row[column.ColumnName]);
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }

            html.Append("</table>");

            return html.ToString();
        }

        public static string ToFileSize(int length)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (length >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                length = length / 1024;
            }

            return String.Format("{0:0.##} {1}", length, sizes[order]);
        }

        public static string ToAlphabet(int value)
        {
            return alphaList.ElementAt(value);
        }

        public static IEnumerable<string> ToKeys(object value)
        {
            if (value != null)
            {
                return PropertyHelper.GetProperties(value).Select(helper => helper.Name);
            }

            return new string[] { };
        }

        public static IDictionary<string, object> ToPairs(object value)
        {
            var pairs = new Dictionary<string, object>();
            if (value != null)
            {
                PropertyHelper.GetProperties(value).Each(helper => pairs.Add(helper.Name, helper.GetValue(value)));
            }

            return pairs;
        }

        public static DataTable ToTable<T>(IEnumerable<T> items) where T : class, new()
        {
            var table = new DataTable();
            ToKeys(new T()).Each(header => table.Columns.Add(header));

            items.Each(item => table.Rows.Add(NewRow<T>(table, item)));

            return table;
        }

        private static DataRow NewRow<T>(DataTable table, T item)
        {
            var row = table.NewRow();
            PropertyHelper.GetProperties(item).Each(helper => row[helper.Name] = helper.GetValue(item));

            return row;
        }
    }
}
