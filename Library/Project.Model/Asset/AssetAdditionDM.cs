using Project.Framework;
using System;

namespace Project.Model
{
    [Serializable]
    public class AssetAdditionDM : DbModel, IDbEntity
    {
        private static DbEntity dbEntity = new DbEntity()
        {
            Name = "AST_Addition",
            PrimaryKeys = "AssetId,Key"
        };

        public Guid AssetId
        { get; set; }

        public string Key
        { get; set; }

        public string Value
        { get; set; }

        public AssetAdditionDM() { }

        public AssetAdditionDM(Guid assetId, string key, string value)
        {
            this.AssetId = assetId;
            this.Key = key;
            this.Value = value;
        }

        public DbEntity GetDbEntity()
        {
            return dbEntity;
        }

        public Guid GetPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public void SetPrimaryKey(Guid key)
        {
            throw new NotImplementedException();
        }
    }
}
