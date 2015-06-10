using System;
using System.Collections.Generic;

namespace Adhesive.Mongodb.Silverlight
{
    public class SearchCondition
    {
        public string SelectedTableName { get; set; }

        public string DatabasePrefix { get; set; }

        public List<string> TableNames { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public Dictionary<string, object> Filters { get; set; }

        public string ContextId { get; set; }
    }
}
