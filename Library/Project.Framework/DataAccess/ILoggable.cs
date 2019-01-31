using System.Data;

namespace Project.Framework
{
    public interface ILoggable
    {
        void Add<T>(T item, ActionType type) where T : class, IDbEntity, IDbModel, new();

        void Add<T>(DataTable table, ActionType type) where T : class, IDbEntity, IDbModel, new();

        void Clear();

        void Record();
    }
}
