
using System;
namespace Adhesive.Alarm.Common
{
    [Serializable]
    public enum AlarmConditionType
    {
        LessThan = 1,
        LessThanAndEqualTo = 2,
        MoreThan = 3,
        MoreThanAndEqualTo = 4,
    }
}
