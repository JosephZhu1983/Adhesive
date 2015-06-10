
namespace Adhesive.Common
{
    public class AdhesiveConfig
    {
        public string ApplicationName { get; set; }

        public bool ClearLocalLogWhenStart { get; set; }

        public LogLevel LocalLoggingServiceLevel { get; set; }

        public string WcfConfigServiceAddress { get; set; }

        public string MongodbServiceAddress { get; set; }

        public string ConfigServiceAddress { get; set; }

        public int StateServiceDefaultReportStateIntervalMilliSeconds { get; set; }
    }
}
