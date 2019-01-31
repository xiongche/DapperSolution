using Project.Core;
using Project.Framework;using System;

namespace Project.Model
{
    [Serializable]
    public class AssetDM : DbModel, IDbEntity, IAsset
    {
        private static DbEntity dbEntity = new DbEntity()
        {
            Name = "AST_Asset",
            PrimaryKeys = "AssetId"
        };

        public DbEntity GetDbEntity()
        {
            return dbEntity;
        }

        public Guid AssetId
        { get; set; }

        public string FileName
        { get; set; }

        public int Type
        { get; set; }

        public string ContentType
        { get; set; }

        public string Extension
        { get; set; }

        public int ContentLength
        { get; set; }

        public bool IsVisible
        { get; set; }

        public void SetPrimaryKey(Guid key)
        {
            this.AssetId = key;
        }

        public Guid GetPrimaryKey()
        {
            return this.AssetId;
        }
    }
}
