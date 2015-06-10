


using System.Threading;

using System.Collections.Generic;

namespace Adhesive.Alarm
{
    internal class AlarmServiceState
    {
        public string AlarmConfigurationItemName { get; set; }

        public Timer CheckTimer { get; set; }

        public Dictionary<string, AlarmServiceStateItem> AlarmServiceStateItems { get; set; }
    }
}
