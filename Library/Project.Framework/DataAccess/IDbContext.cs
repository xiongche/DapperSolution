using System;
using System.Collections.Generic;
using System.Data;

namespace Project.Framework
{
    public interface IDbContext : IDisposable
    {
        bool IsInTran { get; }

        bool IsDirty { get; }

        string Instance { get; }

        void Insert<T>(T item, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        void Update<T>(T item, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        void Delete<T>(object param, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        void BulkInsert<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        void BulkUpdate<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        void BulkDelete<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new();

        R GetValue<T, R>(string by, string field, object param) where T : class, IDbQuery, new();

        R GetValue<T, R>(string by, string field, object param, string[] conditions) where T : class, IDbQuery, new();

        T CheckItem<T>(string by, object param) where T : class, IDbQuery, new();

        T CheckItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new();

        T GetItem<T>(string by, object param) where T : class, IDbQuery, new();

        T GetItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new();

        IList<T> GetTop<T>(string by, int count, object param, string orderBy) where T : class, IDbQuery, new();

        IList<T> GetTop<T>(string by, int count, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new();

        IList<R> GetValues<T, R>(string by, string field, object param, string orderBy = null) where T : class, IDbQuery, new();

        IList<R> GetValues<T, R>(string by, string field, object param, string[] conditions, string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetAll<T>(string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string by, object param, string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string by, object param, string[] conditions, string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string column, DataTable table, string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string column, object param, DataTable table, string orderBy = null) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string column, object param, DataTable table, string[] conditions, string orderBy = null) where T : class, IDbQuery, new();

        //Free Query
        T GetItem<T>(Func<ISqlBuilder> builder);

        T GetItem<T>(object param, Func<ISqlBuilder> builder);

        T GetItem<T>(DataTable table, Func<ISqlBuilder> builder);

        T GetItem<T>(object param, DataTable table, Func<ISqlBuilder> builder);

        R GetValue<R>(Func<ISqlBuilder> builder);

        R GetValue<R>(object param, Func<ISqlBuilder> builder);

        R GetValue<R>(DataTable table, Func<ISqlBuilder> builder);

        R GetValue<R>(object param, DataTable table, Func<ISqlBuilder> builder);

        IList<T> GetItems<T>(Func<ISqlBuilder> builder);

        IList<T> GetItems<T>(object param, Func<ISqlBuilder> builder);

        IList<T> GetItems<T>(DataTable table, Func<ISqlBuilder> builder);

        IList<T> GetItems<T>(object param, DataTable table, Func<ISqlBuilder> builder);

        void BeginTran();

        void Commit();

        void Rollback();
    }
}