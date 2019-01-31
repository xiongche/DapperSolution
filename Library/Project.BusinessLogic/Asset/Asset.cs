using Project.Core;
using Project.Framework;
using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.BusinessLogic
{
    public sealed class Asset : BaseBusinessLogic
    {
        public static string TableName
        {
            get
            {
                return DbEntityCache.GetEntityName<AssetDM>();
            }
        }

        public static AssetType GetAssetType(IAsset asset)
        {
            return GetAssetType(asset.ContentType, asset.Extension);
        }

        public static AssetType GetAssetType(string contentType, string extension)
        {
            var type = GetAssetTypeByContentType(contentType);
            return type == AssetType.Other ? GetAssetTypeByExtension(extension) : type;
        }

        public void Replace(AssetDM item, Guid actedBy)
        {
            DB.Update<AssetDM>(item, actedBy);
        }

        public void Save(AssetDM item, Guid folderId, Guid actedBy)
        {
            if (CheckItemByKey(item) == null)
            {
                DB.Insert<AssetDM>(item, actedBy);
                SaveStructure(item, folderId, actedBy);
            }
            else
            {
                DB.Update<AssetDM>(item, actedBy);
            }
        }

        public void Delete(Guid assetId, Guid actedBy)
        {
            Let<AssetStructure>().Delete(assetId, actedBy);
            DB.Delete<AssetDM>(new { AssetId = assetId }, actedBy);
        }

        public void Rename(Guid assetId, string fileName, Guid actedBy)
        {
            var item = GetItemByAssetId(assetId);
            if (item != null)
            {
                item.FileName = fileName;
                DB.Update<AssetDM>(item, actedBy);
            }
        }

        public AssetDM GetItemByAssetId(Guid assetId)
        {
            return DB.CheckItem<AssetDM>("AssetId", new { AssetId = assetId });
        }

        public IList<AssetDM> GetDirectChildren(Guid folderId)
        {
            var structures = Let<AssetStructure>().GetItemByParentId(folderId, false);
            return GetItemsByAssetIds(structures.Select(model => model.Puid));
        }

        public IList<AssetDM> GetItemsByAssetIds(IEnumerable<Guid> assetIds)
        {
            return DB.GetItems<AssetDM>("AssetId", ScriptHelper.ToTable(assetIds));
        }

        private static AssetType GetAssetTypeByExtension(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case "pdf":
                case "doc":
                case "docx":
                case "ppt":
                case "pptx":
                case "xls":
                case "xlsx":
                    return AssetType.Document;
                case "mp3":
                case "wav":
                    return AssetType.Audio;
                case "mp4":
                case "m4v":
                case "wmv":
                case "webm":
                    return AssetType.Video;
                case "zip":
                case "7z":
                case "rar":
                    return AssetType.Zip;
                default:
                    return AssetType.Other;
            }
        }

        private static AssetType GetAssetTypeByContentType(string contentType)
        {
            switch (contentType.ToLowerInvariant())
            {
                case "image/jpeg":
                case "image/jpg":
                case "image/png":
                case "image/gif":
                    return AssetType.Image;
                case "video/mp4":
                case "video/x-m4v":
                case "video/x-ms-wmv":
                    return AssetType.Video;
                case "audio/mpeg":
                case "audio/wav":
                    return AssetType.Audio;
                case "application/pdf":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                case "application/vnd.ms-excel":
                case "application/msword":
                    return AssetType.Document;
                case "text/html":
                    return AssetType.Html;
                case "application/zip":
                    return AssetType.Zip;
                case "application/octet-stream":
                default:
                    return AssetType.Other;
            }
        }

        private AssetDM CheckItemByKey(AssetDM item)
        {
            return DB.CheckItem<AssetDM>("AssetId", new { AssetId = item.AssetId });
        }

        private void SaveStructure(AssetDM item, Guid folderId, Guid actedBy)
        {
            if (item.IsVisible)
            {
                var folder = Let<AssetFolder>().GetFirstOrDefault(folderId, actedBy);
                Let<AssetStructure>().Join(item.AssetId, folder.FolderId, false, actedBy);
            }
        }
    }
}
