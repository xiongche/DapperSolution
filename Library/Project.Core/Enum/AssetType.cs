using System.ComponentModel;

namespace Project.Core
{
    public enum AssetType
    {
        [Description("Image")]
        Image = 1,

        [Description("Document")]
        Document = 2,

        [Description("Audio")]
        Audio = 3,

        [Description("Video")]
        Video = 4,

        [Description("Zip")]
        Zip = 5,

        [Description("Html")]
        Html = 6,

        [Description("Other")]
        Other = 10000
    }
}
