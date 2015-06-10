using System.Runtime.Serialization;

namespace Adhesive.GeneralPerformance.Common
{
    [DataContract]
    public class GeneralPerformanceInfo
    {
        public int TotalRequestCount { get; set; }

        public long TotalRequestExecutionTime { get; set; }

        public long MaxRequestExecutionTime { get; set; }

        [DataMember]
        public int SuccessRequestCount { get; set; }

        [DataMember]
        public int FailedRequestCount { get; set; }

        [DataMember]
        public int AverageRequestExecutionTime { get; set; }

        [DataMember]
        public string MachineIP { get; set; }

        [DataMember]
        public string PageName { get; set; }

        [DataMember]
        public string AppName { get; set; }
    }
}
