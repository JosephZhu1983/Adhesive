using System.Collections.Generic;

namespace Adhesive.Mongodb.Server.Imp
{
    public class CollectionMetadata
    {
        public string CollectionName { get; set; }

        public List<TextboxFilterColumnInfo> TextboxFilterColumns { get; set; }

        public List<ListFilterColumnInfo> ListFilterColumns { get; set; }

        public List<CascadeFilterColumnInfo> CascadeFilterColumns { get; set; }
    }
}
