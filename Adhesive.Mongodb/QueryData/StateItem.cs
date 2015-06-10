
using System;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class StateItem
    {
        public string DatabaseName { get; set; }

        public DateTime Time { get; set; }

        public long Value { get; set; }

        public string ID { get; set; }
    }
}
