
using System.Runtime.Serialization;
namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbColumnDescription
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public bool IsArrayColumn { get; set; }

        [DataMember]
        public bool IsEntityColumn { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool ShowInTableView { get; set; }

        [DataMember]
        public bool IsTableColumn { get; set; }

        [DataMember]
        public bool IsTimeColumn { get; set; }

        [DataMember]
        public bool IsContextIdentityColumn { get; set; }

        [DataMember]
        public bool IsPrimaryKey { get; set; }

        [DataMember]
        public MongodbIndexOption MongodbIndexOption { get; set; }

        [DataMember]
        public MongodbFilterOption MongodbFilterOption { get; set; }

        [DataMember]
        public MongodbCascadeFilterOption MongodbCascadeFilterOption { get; set; }

        [DataMember]
        public MongodbSortOption MongodbSortOption { get; set; }
    }
}
