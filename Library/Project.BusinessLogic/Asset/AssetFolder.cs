using Project.Core;
using Project.Framework;
using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.BusinessLogic
{
    public sealed class AssetFolder : BaseBusinessLogic
    {
        private const string DEFAULT_FOLDER = "My Assets";

        public static string TableName
        {
            get
            {
                return DbEntityCache.GetEntityName<AssetFolderDM>();
            }
        }

        public void Save(AssetFolderDM item, Guid actedBy)
        {
            Save(item, Guid.Empty, actedBy);
        }

        public void Save(AssetFolderDM item, Guid folderId, Guid actedBy)
        {
            ThrowIfNoFolderName(item);

            item.Name = StringHelper.ToFolderName(item.Name);

            if (item.FolderId.Equals(Guid.Empty))
            {
                item.Code = Let<SerialNo>().GenerateSerialNo(SerialNoType.AssetFolderCode, actedBy);

                DB.Insert<AssetFolderDM>(item, actedBy);
                SaveStructure(item.FolderId, folderId, actedBy);
            }
            else
            {
                DB.Update<AssetFolderDM>(item, actedBy);
            }
        }

        public void Delete(Guid folderId, Guid actedBy)
        {
            Let<AssetStructure>().DeleteByFolderId(folderId, actedBy);
            DB.Delete<AssetFolderDM>(new { FolderId = folderId }, actedBy);
        }

        public void Rename(Guid folderId, string name, Guid actedBy)
        {
            var folder = GetItemByFolderId(folderId);
            if (folder != null)
            {
                folder.Name = StringHelper.ToFolderName(name);
                DB.Update<AssetFolderDM>(folder, actedBy);
            }
        }

        public AssetFolderDM GetRoot(Guid userId)
        {
            var root = DB.GetItem<AssetFolderDM>("Root", new { UserId = userId, IsRoot = true });

            return root == null ? SaveRoot(userId) : root;
        }

        public AssetFolderDM GetItemByFolderId(Guid folderId)
        {
            return DB.GetItem<AssetFolderDM>("FolderId", new { FolderId = folderId });
        }

        public AssetFolderDM GetFirstOrDefault(Guid folderId, Guid userId)
        {
            if (folderId == Guid.Empty)
            {
                return GetRoot(userId);
            }

            var item = GetItemByFolderId(folderId);
            return item == null ? GetRoot(userId) : item;
        }

        public IList<AssetFolderDM> GetItemsByCodes(IEnumerable<string> codes)
        {
            return DB.GetItems<AssetFolderDM>("Code", ScriptHelper.ToTable(codes));
        }

        public IList<AssetFolderDM> GetRecursiveParents(Guid folderId)
        {
            var structure = Let<AssetStructure>().GetItemByPuid(folderId);
            if (structure == null)
            {
                var folder = GetItemByFolderId(folderId);
                return folder == null ? new List<AssetFolderDM>() : new List<AssetFolderDM>() { folder };
            }

            var codes = structure.Path.Split(SymbolConst.PATH_SEPARATOR);
            var folders = GetItemsByCodes(codes);

            return codes.Select(code => folders.FirstOrDefault(model => StringHelper.Compare(model.Code, code))).WhereList(model => model != null);
        }

        private void ThrowIfNoFolderName(AssetFolderDM item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ExpectedException("No folder name.");
            }
        }

        private void SaveStructure(Guid folderId, Guid parentId, Guid actedBy)
        {
            var folder = GetFirstOrDefault(parentId, actedBy);
            Let<AssetStructure>().Join(folderId, folder.FolderId, true, actedBy);
        }

        private AssetFolderDM SaveRoot(Guid userId)
        {
            var root = new AssetFolderDM()
            {
                Name = "My Assets",
                UserId = userId,
                Code = Let<SerialNo>().GenerateSerialNo(SerialNoType.AssetFolderCode, userId),
                IsRoot = true,
            };

            DB.Insert<AssetFolderDM>(root, userId);
            Let<AssetStructure>().SaveRoot(root, userId);

            return root;
        }
    }
}
