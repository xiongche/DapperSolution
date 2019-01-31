using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Project.Framework
{
    public sealed class DbContext : IDbContext
    {
        private string instance;

        private SqlConnection masterConn;
        private SqlTransaction masterTran;

        private IDbMapper dbMapper;
        private IDbMapper GetMapper()
        {
            if (this.dbMapper == null)
            {
                Initialize();
                this.dbMapper = new DbMapper(this.masterConn);
            }

            return this.dbMapper;
        }

        private ILoggable logger;
        private ILoggable GetLogger()
        {
            if (this.logger == null && DbHelper.HasConnection($"{instance}_log"))
            {
                this.logger = new Logger($"{instance}_log");
            }

            return this.logger;
        }

        private bool isInTran = false;
        public bool IsInTran
        {
            get
            {
                return isInTran;
            }
            private set
            {
                isInTran = value;
            }
        }

        private bool isDirty = false;
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            private set
            {
                isDirty = value;
            }
        }

        public string Instance
        {
            get
            {
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public DbContext(string instance)
        {
            this.instance = instance;
        }

        public void BeginTran()
        {
            if (this.isInTran && this.masterTran != null)
            {
                InnerRollback();
            }

            Initialize();

            if (!isInTran)
            {
                this.masterTran = this.masterConn.BeginTransaction();
                this.GetMapper().SetTran(this.masterTran);
            }

            this.isInTran = true;
        }

        public void Commit()
        {
            if (!this.isInTran)
            {
                throw new InvalidOperationException();
            }

            InnerCommit();
            //this.masterConn.Close();
        }

        public void Rollback()
        {
            if (!this.isInTran)
            {
                throw new InvalidOperationException("The SqlTransaction has not began.");
            }

            InnerRollback();
        }

        public void Insert<T>(T item, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            var dbEntity = DbEntityCache.GetDbEntity<T>();
            SetInsertAudit<T>(item, dbEntity, actedBy);

            this.GetMapper().Insert<T>(item);
            this.GetLogger().Add<T>(item, ActionType.Insert);
        }

        public void Update<T>(T item, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            Initialize();
            SetUpdateAudit<T>(item, actedBy);

            if (HashValueHelper.HasChanged(this.masterConn, this.masterTran, item))
            {
                this.GetMapper().Update<T>(item);
                this.GetLogger().Add<T>(item, ActionType.Update);
            }
        }

        public void Delete<T>(object param, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            var item = GetItem<T>("Delete", param);
            if (item != null)
            {
                SetUpdateAudit<T>(item, actedBy);

                this.GetMapper().Delete<T>(item, (IDbModel)item);
                this.GetLogger().Add<T>(item, ActionType.Delete);
            }
        }

        public void BulkInsert<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            if (items.Count() > 0)
            {
                var list = items.ToList();

                var dbEntity = DbEntityCache.GetDbEntity<T>();
                list.Each(item => SetInsertAudit<T>(item, dbEntity, actedBy));

                var table = BulkHelper<T>.ConvertToInsertTable(list);

                this.GetMapper().BulkInsert<T>(table);
                this.GetLogger().Add<T>(table, ActionType.Insert);
            }
        }

        public void BulkUpdate<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            if (items.Count() > 0)
            {
                var list = items.ToList();
                list.Each(item => SetUpdateAudit<T>(item, actedBy));

                var table = BulkHelper<T>.ConvertToUpdateTable(list);

                this.GetMapper().BulkUpdate<T>(table);
                this.GetLogger().Add<T>(table, ActionType.Update);
            }
        }

        public void BulkDelete<T>(IEnumerable<T> items, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            if (items.Count() > 0)
            {
                var list = items.ToList();
                list.Each(item => SetUpdateAudit<T>(item, actedBy));

                var table = BulkHelper<T>.ConvertToDeleteTable(list);

                this.GetMapper().BulkDelete<T>(table);
                this.GetLogger().Add<T>(table, ActionType.Delete);
            }
        }

        public R GetValue<T, R>(string by, string field, object param) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetValue<T, R>(by, field, param, null);
        }

        public R GetValue<T, R>(string by, string field, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetValue<T, R>(by, field, param, conditions);
        }

        public T CheckItem<T>(string by, object param) where T : class, IDbQuery, new()
        {
            return this.GetMapper().CheckItem<T>(by, param, null);
        }

        public T CheckItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            return this.GetMapper().CheckItem<T>(by, param, conditions);
        }

        public T GetItem<T>(string by, object param) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItem<T>(by, param, new string[] { });
        }

        public T GetItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItem<T>(by, param, conditions);
        }

        public IList<T> GetTop<T>(string by, int count, object param, string orderBy) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetTop<T>(by, count, param, null, orderBy);
        }

        public IList<T> GetTop<T>(string by, int count, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetTop<T>(by, count, param, conditions, orderBy);
        }

        public IList<R> GetValues<T, R>(string by, string field, object param, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetValues<T, R>(by, field, param, null, orderBy);
        }

        public IList<R> GetValues<T, R>(string by, string field, object param, string[] conditions, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetValues<T, R>(by, field, param, conditions, orderBy);
        }

        public IList<T> GetAll<T>(string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>("All", new { }, null, orderBy);
        }

        public IList<T> GetItems<T>(string by, object param, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>(by, param, null, orderBy);
        }

        public IList<T> GetItems<T>(string by, object param, string[] conditions, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>(by, param, conditions, orderBy);
        }

        public IList<T> GetItems<T>(string column, DataTable table, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>(column, null, table, null, orderBy);
        }

        public IList<T> GetItems<T>(string column, object param, DataTable table, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>(column, param, table, null, orderBy);
        }

        public IList<T> GetItems<T>(string column, object param, DataTable table, string[] conditions, string orderBy = null) where T : class, IDbQuery, new()
        {
            return this.GetMapper().GetItems<T>(column, param, table, conditions, orderBy);
        }

        public T GetItem<T>(Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItem<T>(null, null, builder().ToString());
        }

        public T GetItem<T>(object param, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItem<T>(param, null, builder().ToString());
        }

        public T GetItem<T>(DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItem<T>(null, table, builder().ToString());
        }

        public T GetItem<T>(object param, DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItem<T>(param, table, builder().ToString());
        }

        public R GetValue<R>(Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetValue<R>(null, null, builder().ToString());
        }

        public R GetValue<R>(object param, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetValue<R>(param, null, builder().ToString());
        }

        public R GetValue<R>(DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetValue<R>(null, table, builder().ToString());
        }

        public R GetValue<R>(object param, DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetValue<R>(param, table, builder().ToString());
        }

        public IList<T> GetItems<T>(Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItems<T>(null, null, builder().ToString());
        }

        public IList<T> GetItems<T>(object param, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItems<T>(param, null, builder().ToString());
        }

        public IList<T> GetItems<T>(DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItems<T>(null, table, builder().ToString());
        }

        public IList<T> GetItems<T>(object param, DataTable table, Func<ISqlBuilder> builder)
        {
            return this.GetMapper().GetItems<T>(param, table, builder().ToString());
        }

        private void Initialize()
        {
            if (this.masterConn == null)
            {
                this.masterConn = DbHelper.GetConnection(instance);
            }
        }

        private void InnerCommit()
        {
            this.isInTran = false;
            this.masterTran.Commit();
            this.masterTran.Dispose();
            this.masterTran = null;
            this.GetMapper().SetTran(this.masterTran);
        }

        private void InnerRollback()
        {
            this.isInTran = false;
            this.masterTran.Rollback();
            this.masterTran.Dispose();
            this.masterTran = null;
            this.GetMapper().SetTran(this.masterTran);
            this.GetLogger().Clear();
        }

        private void SetInsertAudit<T>(T item, DbEntity dbEntity, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            if (dbEntity.IsNewSequentialPrimaryKey && item.GetPrimaryKey() == Guid.Empty)
            {
                item.SetPrimaryKey(Guid.NewGuid());
            }

            item.HashValue = HashValueHelper.Generate<T>(item);
            item.CreatedBy = actedBy;
            item.CreatedDate = DateTimeHelper.Now();
        }

        private void SetUpdateAudit<T>(T item, Guid actedBy) where T : class, IDbEntity, IDbModel, new()
        {
            item.HashValue = HashValueHelper.Generate<T>(item);
            item.UpdatedBy = actedBy;
            item.UpdatedDate = DateTimeHelper.Now();
        }

        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                Dispose(masterTran);
                Dispose(masterConn);
                Dispose(dbMapper);

                if (this.logger != null)
                {
                    this.logger.Record();
                }
            }

            this.disposed = true;
        }

        private void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}