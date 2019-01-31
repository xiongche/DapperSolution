using Project.Core;
using Project.Framework;
using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.BusinessLogic
{
    public sealed class SerialNo : BaseBusinessLogic
    {
        public string GenerateSerialNo(SerialNoType type, Guid actedBy)
        {
            var item = GetItemByType((int)type);

            item.Count++;
            Save(item, actedBy);

            return item.GetNo();
        }

        private void Save(SerialNoDM item, Guid actedBy)
        {
            if (item.SerialNoId.Equals(Guid.Empty))
            {
                DB.Insert<SerialNoDM>(item, actedBy);
            }
            else
            {
                DB.Update<SerialNoDM>(item, actedBy);
            }
        }

        private SerialNoDM GetItemByType(int type)
        {
            var items = GetItemsByType(type);
            if (items.Count == 0)
            {
                throw new ExpectedException("No serial No configuration in the system.");
            }

            var item = items.OrderByDescending(i => i.Year).OrderByDescending(i => i.Month).First();
            if (item.Mode == (int)SerialNoMode.Year)
            {
                return GetItemByYearlyMode(item);
            }
            if (item.Mode == (int)SerialNoMode.Month)
            {
                return GetItemByMonthlyMode(item);
            }

            return item;
        }

        private SerialNoDM GetItemByYearlyMode(SerialNoDM item)
        {
            int year = DateTimeHelper.Now().Year;
            if (item.Year.HasValue && item.Year.Value.Equals(year))
            {
                return item;
            }

            item.SerialNoId = Guid.Empty;
            item.Year = year;
            item.Count = 0;

            Save(item, Guid.Empty);

            return item;
        }

        private SerialNoDM GetItemByMonthlyMode(SerialNoDM item)
        {
            int year = DateTimeHelper.Now().Year;
            int month = DateTimeHelper.Now().Month;
            if (item.Year.HasValue && item.Year.Value.Equals(year) && item.Month.HasValue && item.Year.Value.Equals(month))
            {
                return item;
            }

            item.SerialNoId = Guid.Empty;
            item.Year = year;
            item.Month = month;
            item.Count = 0;

            Save(item, Guid.Empty);

            return item;
        }

        private IList<SerialNoDM> GetItemsByType(int type)
        {
            return DB.GetItems<SerialNoDM>("Type", new { Type = type });
        }
    }
}
