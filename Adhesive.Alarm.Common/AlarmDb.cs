using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Adhesive.Domain;

namespace Adhesive.Alarm.Common
{
    [DataContract]
    public class AlarmItem : Entity
    {
        [DataMember]
        public string AlarmConfigName { get; set; }

        [DataMember]
        public string AlarmDatabaseName { get; set; }

        [DataMember]
        public string AlarmTableName { get; set; }

        public int AlarmStatusId { get; set; }

        [DataMember]
        [NotMapped]
        public AlarmStatus AlarmStatus
        {
            get { return (AlarmStatus)AlarmStatusId; }
            set { AlarmStatusId = (int)value; }
        }

        [DataMember]
        public List<AlarmProcessItem> AlarmProcessItems { get; set; }

        [DataMember]
        public DateTime OpenTime { get; set; }

        [DataMember]
        public DateTime HandleTime { get; set; }

        [DataMember]
        public DateTime CloseTime { get; set; }

        [DataMember]
        public int AlarmTimes { get; set; }
    }

    [DataContract]
    public class AlarmProcessItem : Entity
    {
        [DataMember]
        public AlarmItem AlarmItem { get; set; }

        [DataMember]
        public string AlarmItemId { get; set; }

        public int AlarmStatusId { get; set; }

        [DataMember]
         [NotMapped]
        public AlarmStatus AlarmStatus
        {
            get { return (AlarmStatus)AlarmStatusId; }
            set { AlarmStatusId = (int)value; }
        }

        [DataMember]
        public string MobileComment { get; set; }

        [DataMember]
        public string MailComment { get; set; }

        [DataMember]
        public string ProcessUserName { get; set; }

        [DataMember]
        public string ProcessUserRealName { get; set; }

        [DataMember]
        public DateTime EventTime { get; set; }

    }
}