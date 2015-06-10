
using System.Collections.Generic;
namespace Adhesive.Mongodb
{
    public class Detail
    {
        public string ColumnName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public List<Detail> SubDetails { get; set; }
    }
}
