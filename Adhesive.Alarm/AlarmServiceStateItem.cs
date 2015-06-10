
using System;

namespace Adhesive.Alarm
{
    internal class AlarmServiceStateItem
    {
        public string ReceiverGroupName { get; set; }

        public DateTime AlarmReceiverGroupLastMailMessageTime { get; set; }

        public DateTime AlarmReceiverGroupLastMobileMessageTime { get; set; }
    }
}
