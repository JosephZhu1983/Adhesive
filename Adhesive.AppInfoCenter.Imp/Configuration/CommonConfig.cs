
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "基本配置")]
    public class CommonConfig
    {
        [ConfigItem(FriendlyName = "是否嵌入基本信息到页面")]
        public bool EmbedInfoToPage { get; set; }
    }

}
