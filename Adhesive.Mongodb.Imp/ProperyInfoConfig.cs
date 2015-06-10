
namespace Adhesive.Mongodb.Imp
{
    internal class ProperyInfoConfig
    {
        public bool IsCascadeFilterLevelOne { get; set; }

        public bool IsCascadeFilterLevelTwo { get; set; }

        public bool IsCascadeFilterLevelThree { get; set; }

        public bool IsDateColumn { get; set; }

        public bool IsTableName { get; set; }

        public bool IsIgnore { get; set; }

        public string ColumnName { get; set; }
    }
}
