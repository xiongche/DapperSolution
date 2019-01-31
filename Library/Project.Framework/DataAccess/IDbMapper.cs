using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Project.Framework
{
    public interface IDbMapper : IDisposable
    {
        void SetTran(SqlTransaction tran);

        void Insert<T>(T item) where T : class, IDbEntity, IDbModel, new();

        void Update<T>(T item) where T : class, IDbEntity, IDbModel, new();

        void Delete<T>(object param, IDbModel dbModel) where T : class, IDbEntity, IDbModel, new();

        void BulkInsert<T>(DataTable table) where T : class, IDbEntity, IDbModel, new();

        void BulkUpdate<T>(DataTable table) where T : class, IDbEntity, IDbModel, new();

        void BulkDelete<T>(DataTable table) where T : class, IDbEntity, IDbModel, new();

        T GetItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new();

        T CheckItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new();

        R GetValue<T, R>(string by, string field, object param, string[] conditions) where T : class, IDbQuery, new();

        IList<T> GetTop<T>(string by, int count, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new();

        IList<R> GetValues<T, R>(string by, string field, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string by, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new();

        IList<T> GetItems<T>(string column, object param, DataTable table, string[] conditions, string orderBy) where T : class, IDbQuery, new();

        R GetValue<R>(object param, DataTable table, string sql);

        T GetItem<T>(object param, DataTable table, string sql);

        IList<T> GetItems<T>(object param, DataTable table, string sql);
    }
}
