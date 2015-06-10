using System.Collections.Generic;
namespace Adhesive.Mongodb.Silverlight
{
    public class StateCondition
    {
        public string DatabasePrefix { get; set; }

        public string TableName { get; set; }

        public Dictionary<string, object> Filters { get; set; }
    }
}
