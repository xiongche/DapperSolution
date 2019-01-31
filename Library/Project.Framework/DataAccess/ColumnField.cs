using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Framework
{
    internal class ColumnField
    {
        public string Name
        { get; set; }

        public bool IsNullable
        { get; set; }

        internal ColumnField(PropertyHelper helper)
        {
            var type = helper.Type.IsGenericType ? helper.Type.GetGenericTypeDefinition() : helper.Type;

            this.Name = helper.Name;
            this.IsNullable = !helper.Type.IsValueType ? true : type == typeof(Nullable<>);
        }

        internal static IEnumerable<ColumnField> GetFields(object param)
        {
            if (param == null)
            {
                return new ColumnField[] { };
            }

            return PropertyHelper.GetProperties(param).Select(helper => new ColumnField(helper));
        }
    }
}
