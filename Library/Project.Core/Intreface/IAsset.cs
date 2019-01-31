using System;

namespace Project.Core
{
    public interface IAsset
    {
        Guid AssetId { get; set; }

        int Type { get; set; }

        int ContentLength { get; set; }

        bool IsVisible { get; set; }

        string FileName { get; set; }

        string Extension { get; set; }

        string ContentType { get; set; }        
    }
}
