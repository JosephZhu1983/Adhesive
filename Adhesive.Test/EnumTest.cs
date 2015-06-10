
using System;
using System.ComponentModel;
namespace Adhesive.Test
{
    [Description("交易方式")]
    public enum TradingServiceType
    {
        [Description("担保")]
        Escort = 1,
        [Description("寄售")]
        Consignment = 2,
        [Description("账号")]
        Id = 3,
        [Description("网页游戏")]
        WebGame = 4
    }
    [Flags]
    public enum Styles
    {
        ShowBorder = 1,        
        ShowCaption = 2,        
        ShowToolbox = 4        
    } 
}
