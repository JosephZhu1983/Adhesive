using System;
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.Test
{
    /// <summary>
    /// 卖家信用等级
    /// </summary>
    public enum UserGrade
    {
        Init = 0,//初始状态，未转换
        None = 999,//无
        Star1 = 1,//1星
        Star2 = 2,
        Star3 = 3,
        Star4 = 4,
        Star5 = 5,
        Diamond1 = 11,//1钻
        Diamond2 = 12,
        Diamond3 = 13,
        Diamond4 = 14,
        Diamond5 = 15,
        Crown1 = 21,//1皇冠
        Crown2 = 22,
        Crown3 = 23,
        Crown4 = 24,
        Crown5 = 25,
    }

    /// <summary>
    /// 商品推广配置信息
    /// </summary>
    [ConfigEntity(FriendlyName = "商品推广配置信息")]
    public class BizOfferPromotionConfig
    {
        /// <summary>
        /// 开关
        /// </summary>
        [ConfigItem(FriendlyName = "开关")]
        public bool OnOff { get; set; }

        /// <summary>
        /// 推广联接
        /// </summary>
        [ConfigItem(FriendlyName = "推广联接")]
        public string PromotionUrl { get; set; }

        private List<string> _promotionUsers;
        /// <summary>
        /// 推广用户
        /// </summary>
        [ConfigItem(FriendlyName = "推广用户")]
        public List<string> PromotionUsers
        {
            get
            {
                if (_promotionUsers == null)
                {
                    _promotionUsers = new List<string>();
                }
                return _promotionUsers;
            }
            set
            {
                _promotionUsers = value;
            }
        }


        private PromotionPrice _promotionPrice;

        /// <summary>
        /// 推扩价格,未配置返回默认
        /// </summary>
        [ConfigItem(FriendlyName="推广价格")]
        public PromotionPrice PromotionPrice
        {
            get
            {
                if (_promotionPrice == null)
                {
                    _promotionPrice = new PromotionPrice();
                }
                return _promotionPrice;
            }
            set
            {
                _promotionPrice = value;
            }
        }


        private List<PromotionTime> _promotionTimes;

        /// <summary>
        /// 推扩时间段,未配置返回默认
        /// </summary>
        [ConfigItem(FriendlyName = "推扩时间段")]
        public List<PromotionTime> PromotionTimes
        {
            get
            {
                if (_promotionTimes == null)
                {
                    _promotionTimes = new List<PromotionTime>();
                    _promotionTimes.Add(new PromotionTime("T1", false, 0, 2));
                    _promotionTimes.Add(new PromotionTime("T2", false, 2, 4));
                    _promotionTimes.Add(new PromotionTime("T3", false, 4, 6));
                    _promotionTimes.Add(new PromotionTime("T4", false, 6, 8));
                    _promotionTimes.Add(new PromotionTime("T5", false, 8, 10));
                    _promotionTimes.Add(new PromotionTime("T6", false, 10, 12));
                    _promotionTimes.Add(new PromotionTime("T7", false, 12, 14));
                    _promotionTimes.Add(new PromotionTime("T8", false, 14, 16));
                    _promotionTimes.Add(new PromotionTime("T9", false, 16, 18));
                    _promotionTimes.Add(new PromotionTime("T10", false, 18, 20));
                    _promotionTimes.Add(new PromotionTime("T11", false, 20, 22));
                    _promotionTimes.Add(new PromotionTime("T12", false, 22, 24));
                    _promotionTimes.Add(new PromotionTime("T13", true, 0, 2));
                    _promotionTimes.Add(new PromotionTime("T14", true, 2, 4));
                    _promotionTimes.Add(new PromotionTime("T15", true, 4, 6));
                    _promotionTimes.Add(new PromotionTime("T16", true, 6, 8));

                }
                return _promotionTimes;
            }
            set
            {
                _promotionTimes = value;
            }

        }

        
        private List<UserGrade> _userGrades;

        /// <summary>
        /// 开通的信用等级
        /// </summary>
        [ConfigItem(FriendlyName = "开通的信用等级")]
        public List<UserGrade> UserGrades
        {
            get
            {
                if (_userGrades == null)
                {
                    _userGrades = new List<UserGrade>();
                }
                return _userGrades;
            }
            set
            {
                _userGrades = value;
            }
        }
    }

    /// <summary>
    /// 推扩价格
    /// </summary>
    public class PromotionPrice
    {
        /// <summary>
        /// 游戏级价格
        /// </summary>
        [ConfigItem(FriendlyName = "游戏级价格")]
        public decimal Game { get; set; }

        /// <summary>
        /// 物品类型级价格
        /// </summary>
        [ConfigItem(FriendlyName = "物品类型级价格")]
        public decimal BizOfferType { get; set; }

        /// <summary>
        /// 游戏区级价格
        /// </summary>
        [ConfigItem(FriendlyName = "游戏区级价格")]
        public decimal GameArea { get; set; }

        /// <summary>
        /// 游戏服级价格
        /// </summary>
        [ConfigItem(FriendlyName = "游戏服级价格")]
        public decimal GameServer { get; set; }

        /// <summary>
        /// 热点时间级价格
        /// </summary>
        [ConfigItem(FriendlyName = "热点时间级价格")]
        public decimal HotTime { get; set; }
    }

    /// <summary>
    /// 推扩时间段
    /// </summary>
    public class PromotionTime
    {
        public PromotionTime()
        {

        }


        public PromotionTime(string id, bool isNextDay, int startHour, int endHour)
        {
            Id = id;
            IsNextDay = isNextDay;
            int day = IsNextDay ? 1 : 0;
            StartTime = new TimeSpan(day, startHour, 0, 0);
            if (endHour < 24)
            {
                EndTime = new TimeSpan(day, endHour, 0, 0);
            }
            else
            {
                EndTime = new TimeSpan(1, 00, 00, 00);
            }
        }

        /// <summary>
        /// 时间区段编号
        /// </summary>
        [ConfigItem(FriendlyName = "时间区段编号")]
        public string Id { get; set; }

        /// <summary>
        /// 时间区段格式化名称
        /// </summary>
        [ConfigItem(FriendlyName = "时间区段格式化名称")]
        public string Name
        {
            get
            {
                if (EndTime.Days == 1 && EndTime.Hours == 0 && EndTime.Seconds == 0)
                {
                    return string.Format("{0}:{1}-{2}:{3}", TimeToString(StartTime.Hours), TimeToString(StartTime.Minutes), TimeToString(24), TimeToString(0));
                }
                return string.Format("{0}:{1}-{2}:{3}", TimeToString(StartTime.Hours), TimeToString(StartTime.Minutes), TimeToString(EndTime.Hours), TimeToString(EndTime.Minutes));
            }
        }


        /// <summary>
        /// 开始时间
        /// </summary>
        [ConfigItem(FriendlyName = "开始时间")]
        public TimeSpan StartTime { get; private set; }


        public long StartTimeTicks
        {
            get
            {
                return StartTime.Ticks;
            }
            set
            {
                StartTime = new TimeSpan(value);
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ConfigItem(FriendlyName = "结束时间")]
        public TimeSpan EndTime { get; private set; }

        public long EndTimeTicks
        {
            get
            {
                return EndTime.Ticks;
            }
            set
            {
                EndTime = new TimeSpan(value);
            }
        }

        /// <summary>
        /// 是否是热点时间
        /// </summary>
        [ConfigItem(FriendlyName = "是否是热点时间")]
        public bool IsHot { get; set; }

        /// <summary>
        /// 是否是次日
        /// </summary>
        [ConfigItem(FriendlyName = "是否是次日")]
        public bool IsNextDay { get; set; }

        private string TimeToString(int i)
        {
            if (i >= 0 && i <= 9)
            {
                return string.Format("0{0}", i);
            }
            else
            {
                return i.ToString();
            }
        }
        public override string ToString()
        {
            return string.Format("Id :{0},Name:{1},StartTime:{2},StartTimeTicks :{3},EndTime :{4},EndTimeTicks :{5},IsHot :{6},IsNextDay :{7}", Id, Name, StartTime, StartTimeTicks, EndTime, EndTimeTicks, IsHot, IsNextDay);
        }
    }

}
