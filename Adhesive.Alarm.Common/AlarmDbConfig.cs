using System.Data.Entity.ModelConfiguration;

namespace Adhesive.Alarm.Common
{
    public class AlarmItemConfiguration : EntityTypeConfiguration<AlarmItem>
    {
        public AlarmItemConfiguration()
            : base()
        {
            Property(e => e.AlarmConfigName).IsRequired().HasMaxLength(200);
            Property(e => e.AlarmDatabaseName).IsRequired().HasMaxLength(200);
            Property(e => e.AlarmTableName).IsRequired().HasMaxLength(200);
            Property(e => e.AlarmStatusId).IsRequired();
            Property(e => e.OpenTime).IsOptional().HasColumnType("datetime2");
            Property(e => e.HandleTime).IsOptional().HasColumnType("datetime2");
            Property(e => e.CloseTime).IsOptional().HasColumnType("datetime2");
            HasMany(e => e.AlarmProcessItems).WithRequired(e => e.AlarmItem).HasForeignKey(e => e.AlarmItemId);
        }
    }

    public class AlarmProcessItemConfiguration : EntityTypeConfiguration<AlarmProcessItem>
    {
        public AlarmProcessItemConfiguration()
            : base()
        {
            Property(e => e.AlarmStatusId).IsRequired();
            Property(e => e.MailComment).HasMaxLength(500).IsOptional();
            Property(e => e.MobileComment).HasMaxLength(50);
            Property(e => e.ProcessUserName).HasMaxLength(50).IsOptional();
            Property(e => e.ProcessUserRealName).HasMaxLength(50).IsOptional();
            Property(e => e.EventTime).IsRequired().HasColumnType("datetime2");
        }
    }
}
