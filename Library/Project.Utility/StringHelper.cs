using HtmlAgilityPack;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public sealed class StringHelper
    {
        private static string[] ValidAttributes = new string[] { "STYLE", "SRC", "ALT", "HREF", "BORDER", "CELLPADDING", "CELLSPACING" };

        public const string CAPITAL_LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string LinuxNewLine = "\n";

        public static bool Compare(string value1, string value2)
        {
            return string.Equals(ToNoSpace((object)value1), ToNoSpace((object)value2), StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(string value, string contain)
        {
            return (value == null || contain == null) ? false : ToPureUpper(value).Contains(ToPureUpper(contain));
        }

        public static bool Contains(IEnumerable<string> values, string contain)
        {
            return values.Any(value => Compare(value, contain));
        }

        public static bool HasValue(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return false;
            }

            return row.IsNull(columnName) ? false : !string.IsNullOrEmpty(row[columnName].ToString());
        }

        public static bool IsNullOrEmpty(object value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.ToString());
        }

        public static string Fill(string template, object value)
        {
            var pairs = ConvertHelper.ToPairs(value);
            foreach (var key in pairs.Keys)
            {
                var text = pairs[key] == null ? string.Empty : pairs[key].ToString();
                template = Regex.Replace(template, $"##{key}##".Trim(), text, RegexOptions.IgnoreCase);
            }

            return template;
        }

        public static string Reverse(string value)
        {
            var chars = value.ToCharArray();
            Array.Reverse(chars);

            return new string(chars);
        }

        public static string ToPureLowerNoSpace(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Replace(" ", "").ToLowerInvariant();
        }

        public static string ToPureLowerNoSpace(object value)
        {
            return value == null ? string.Empty : value.ToString().Replace(" ", "").ToLowerInvariant();
        }

        public static string ToPureUpperNoSpace(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Replace(" ", "").ToUpperInvariant();
        }

        public static string ToPureUpperNoSpace(object value)
        {
            return value == null ? string.Empty : value.ToString().Replace(" ", "").ToUpperInvariant();
        }

        public static string ToPureUpper(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.ToUpperInvariant();
        }

        public static string ToPureUpper(object value)
        {
            return value == null ? string.Empty : value.ToString().ToUpperInvariant();
        }

        public static string ToRemoveExtraSpace(string value)
        {
            var whiteSpace = new char[] { ' ' };
            var split = value.Split(whiteSpace, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(" ", split);
        }

        public static string ToSafeString(string value)
        {
            return string.IsNullOrEmpty(value) ? value : ToNoHtml(value);
        }

        public static string ToSafeString(object value)
        {
            if (value == null)
            {
                return null;
            }

            return ToSafeString(value.ToString());
        }

        public static string ToSafeHtml(string value)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(value);

            var invalidNodes = doc.DocumentNode.SelectNodes("//applet|//body|//embed|//frame|//script|//frameset|//iframe|//layer|//link|//ilayer|//meta|//object");
            if (invalidNodes != null)
            {
                for (int i = 0, j = invalidNodes.Count; i < j; i++)
                {
                    invalidNodes[i].ParentNode.RemoveChild(invalidNodes[i]);
                }
            }

            ClearUnsafeHtmlNode(doc.DocumentNode);

            if (!string.IsNullOrEmpty(doc.DocumentNode.OuterHtml))
            {
                return doc.DocumentNode.OuterHtml.Replace("&nbsp;", " ");
            }

            return doc.DocumentNode.OuterHtml;
        }

        public static string ToUserName(string value)
        {
            return ToPureLowerNoSpace(value);
        }

        public static string ToUserName(object value)
        {
            return value == null ? null : ToPureLowerNoSpace(value);
        }

        public static string ToSubstring(string value, int length)
        {
            return value.Length > length ? value.Substring(0, length) + "..." : value;
        }

        public static string ToNoSpace(object value)
        {
            return value == null ? string.Empty : value.ToString().Replace(" ", "");
        }

        public static string ToNoSpace(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Replace(" ", "");
        }

        public static string ToHtml(string value)
        {
            return value.Replace(System.Environment.NewLine, @"<br />").Replace(LinuxNewLine, @"<br />");
        }

        public static string ToPartialHtml(string content)
        {
            content = content.Length > 300 ? content.Substring(0, 300) + "..." : content;

            return content.Replace(Environment.NewLine, @"<br />");
        }

        public static string ToNoHtml(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = Regex.Replace(value, "<.*?>", String.Empty);
            value = Regex.Replace(value, "&lt;.*?&gt", String.Empty);

            return value;
        }

        public static string ToFileName(string value)
        {
            value = value.Replace(" ", "");
            char[] invalidChars = Path.GetInvalidFileNameChars();
            for (int i = 0, j = invalidChars.Length; i < j; i++)
            {
                value = value.Replace(invalidChars[i], '_');
            }

            return value;
        }

        public static string ToFolderName(string value)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            for (int i = 0, j = invalidChars.Length; i < j; i++)
            {
                value = value.Replace(invalidChars[i], '_');
            }

            return value;
        }

        public static string ToIntegralString(int input, int length)
        {
            return input.ToString(string.Format("D{0}", length));
        }

        private static void ClearUnsafeHtmlNode(HtmlNode htmlNode)
        {
            var attributes = htmlNode.Attributes.Where(attribute => !StringHelper.Contains(ValidAttributes, attribute.Name));
            attributes.Each(attribute => htmlNode.Attributes.Remove(attribute));

            if (htmlNode.HasChildNodes)
            {
                htmlNode.ChildNodes.Each(childNode => ClearUnsafeHtmlNode(childNode));
            }
        }
    }
}
