
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Adhesive.Config
{
    [DataContract]
    public class ConfigItem
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string ParentId { get; set; }
        [DataMember]
        public ConfigItem Parent { get; set; }
        [DataMember]
        public List<ConfigItem> ChildItems { get; set; }
        [DataMember]
        public string AppName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FriendlyName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public byte[] RowVersion { get; set; }
        [DataMember]
        //[NotMapped]
        public object ObjectValue { get; set; }
        [DataMember]
        public string SourceId { get; set; }
        [DataMember]
        public string ValueType { get; set; }
        [DataMember]
        public string ValueTypeEnum { get; set; }
        [DataMember]
        public bool IsCompositeValue { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public bool ItemsInited { get; set; }
        public ConfigItem()
        {
            CreatedOn = DateTime.Now;
            ChildItems = new List<ConfigItem>();
            IsDeleted = false;
            ItemsInited = false;
        }
        public override bool Equals(object obj)
        {
            ConfigItem configItem = (ConfigItem)obj;
            if (configItem.Id == this.Id)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
