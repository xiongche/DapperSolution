using Project.Core;
using Project.Framework;
using Project.Model;
using System;
using System.Collections.Generic;

namespace Project.BusinessLogic
{
    public sealed class AssetStructure : BaseBusinessLogic
    {
        public static string TableName
        {
            get
            {
                return DbEntityCache.GetEntityName<AssetStructureDM>();
            }
        }

        public void SaveRoot(AssetFolderDM folder, Guid actedBy)
        {
            if (folder.IsRoot)
            {
                DB.Insert<AssetStructureDM>(new AssetStructureDM(folder.FolderId, Guid.Empty, true, 1, folder.Code), actedBy);
            }
        }

        public void Join(Guid puid, Guid folderId, bool isFolder, Guid actedBy)
        {
            InnerJoin(new AssetStructureDM(puid, folderId, isFolder), folderId, actedBy);
        }

        public void Delete(Guid puid, Guid actedBy)
        {
            DB.Delete<AssetStructureDM>(new { Puid = puid }, actedBy);
        }

        public void DeleteByFolderId(Guid folderId, Guid actedBy)
        {
            var children = GetRecursivChildren(folderId, true);
            DB.BulkDelete<AssetStructureDM>(children, actedBy);
        }

        public void Move(Guid puid, Guid destinationId, Guid actedBy)
        {

            var item = GetItemByPuid(puid);
            if (!item.IsFolder)
            {
                InnerJoin(item, destinationId, actedBy);
            }
            else
            {
                MoveFolder(item, destinationId, actedBy);
            }
        }

        public string GetPath(Guid folderId)
        {
            var item = GetItemByPuid(folderId);
            if (item != null)
            {
                return item.Path;
            }

            var folder = Let<AssetFolder>().GetItemByFolderId(folderId);
            if (folder != null)
            {
                return folder.Code;
            }

            throw new ExpectedException("No structure path.");
        }

        public string GetPathCondition(Guid folderId, bool includeSelf)
        {
            var path = GetPath(folderId);

            return includeSelf ? path : $"{path}{SymbolConst.PATH_SEPARATOR}";
        }

        public AssetStructureDM GetItemByPuid(Guid puid)
        {
            return DB.GetItem<AssetStructureDM>("Puid", new { Puid = puid });
        }

        public IList<AssetStructureDM> GetItemByParentId(Guid parentId, bool isFolder)
        {
            return DB.GetItems<AssetStructureDM>("ParentId", new { ParentId = parentId, IsFolder = isFolder });
        }

        public IList<AssetStructureDM> GetRecursivChildren(Guid folderId, bool includeSelf)
        {
            var conditions = new string[] { "Path LIKE @Path+'%'" };
            var path = GetPathCondition(folderId, includeSelf);

            return DB.GetItems<AssetStructureDM>("RecursiveFolders", new { Path = path }, conditions);
        }

        private void Save(AssetStructureDM item, Guid actedBy)
        {
            if (CheckItemByKey(item) == null)
            {
                DB.Insert<AssetStructureDM>(item, actedBy);
            }
            else
            {
                DB.Update<AssetStructureDM>(item, actedBy);
            }
        }

        private void InnerJoin(AssetStructureDM item, Guid folderId, Guid actedBy)
        {
            item.ParentId = folderId;
            SetPath(item, folderId);

            Save(item, actedBy);
        }

        private void MoveFolder(AssetStructureDM item, Guid destinationId, Guid actedBy)
        {
            if (item.Puid == destinationId || item.ParentId == destinationId)
            {
                return;
            }

            var destination = GetItemByPuid(destinationId);
            if (destination.Path.Contains(item.Path))
            {
                return;
            }

            var path = item.Path;
            var children = GetRecursiveChildren(item.Puid, false);
            var destinationPath = $"{destination.Path}{SymbolConst.PATH_SEPARATOR}{GetLastCode(path)}";

            item.ParentId = destinationId;
            item.Path = destinationPath;
            item.Level = destinationPath.Split(SymbolConst.PATH_SEPARATOR).Length;
            DB.Update<AssetStructureDM>(item, actedBy);

            foreach (var child in children)
            {
                child.Path = child.Path.Replace(path, destinationPath);
                child.Level = child.Path.Split(SymbolConst.PATH_SEPARATOR).Length;
            }

            DB.BulkUpdate<AssetStructureDM>(children, actedBy);
        }

        private string GetLastCode(string path)
        {
            var codes = path.Split(SymbolConst.PATH_SEPARATOR);

            return codes.Length > 0 ? codes[codes.Length - 1] : string.Empty;
        }

        private AssetStructureDM CheckItemByKey(AssetStructureDM item)
        {
            return DB.CheckItem<AssetStructureDM>("Puid", new { Puid = item.Puid });
        }

        private IList<AssetStructureDM> GetRecursiveChildren(Guid folderId, bool includeSelf)
        {
            var conditions = new string[] { "Path LIKE @Path+'%'" };
            var path = GetPathCondition(folderId, includeSelf);

            return DB.GetItems<AssetStructureDM>("RecursiveChildren", new { Path = path }, conditions);
        }

        private void SetPath(AssetStructureDM item, Guid folderId)
        {
            var path = GetPath(folderId);
            if (!item.IsFolder)
            {
                item.Path = $"{path}{SymbolConst.PATH_SEPARATOR}Asset";
            }
            else
            {
                var folder = Let<AssetFolder>().GetItemByFolderId(item.Puid);
                item.Path = $"{path}{SymbolConst.PATH_SEPARATOR}{folder.Code}";
            }

            item.Level = item.Path.Split(SymbolConst.PATH_SEPARATOR).Length;
        }
    }
}