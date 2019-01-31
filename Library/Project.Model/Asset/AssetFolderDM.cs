using Project.Framework;using System;

namespace Project.Model
{
    [Serializable]
    public class AssetFolderDM : DbModel, IDbEntity
    {
        private static DbEntity dbEntity = new DbEntity()
        {
            Name = "AST_Folder",
            PrimaryKeys = "FolderId",
            IsNewSequentialPrimaryKey = true
        };

        public Guid FolderId
        { get; set; }

        public Guid UserId
        { get; set; }

        public string Name
        { get; set; }

        public string Code
        { get; set; }

        public bool IsRoot
        { get; set; }

        public AssetFolderDM() { }

        public AssetFolderDM(Guid userId, string name)
        {
            this.UserId = userId;
            this.Name = name;
        }

        public DbEntity GetDbEntity()
        {
            return dbEntity;
        }

        public Guid GetPrimaryKey()
        {
            return this.FolderId;
        }

        public void SetPrimaryKey(Guid key)
        {
            this.FolderId = ConvertHelper.ToGuid(key);
        }
    }
}
