using Project.Framework;using System;

namespace Project.Model
{
    [Serializable]
    public class AssetStructureDM : DbModel, IDbEntity
    {
        private static DbEntity dbEntity = new DbEntity()
        {
            Name = "AST_Structure",
            PrimaryKeys = "Puid"
        };

        public Guid Puid { get; set; }

        public Guid ParentId { get; set; }

        public int Position { get; set; }

        public bool IsFolder { get; set; }

        public string Path { get; set; }

        public int Level { get; set; }

        public AssetStructureDM() { }

        public AssetStructureDM(Guid puid, Guid parentId, bool isFolder)
        {
            this.Puid = puid;
            this.ParentId = parentId;
            this.IsFolder = isFolder;
        }

        public AssetStructureDM(Guid puid, Guid parentId, bool isFolder, int level, string path)
        {
            this.Puid = puid;
            this.ParentId = parentId;
            this.IsFolder = IsFolder;
            this.Level = level;
            this.Path = path;
        }

        public DbEntity GetDbEntity()
        {
            return dbEntity;
        }

        public void SetPrimaryKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public Guid GetPrimaryKey()
        {
            throw new NotImplementedException();
        }
    }
}
