

using System.Data.Entity.ModelConfiguration;

namespace Adhesive.Config.Server
{
    public class ConfigItemConfiguration : EntityTypeConfiguration<ConfigItem>
    {
        public ConfigItemConfiguration()
            : base()
        {
            HasKey(e => e.Id);
            Property(e => e.Id).HasMaxLength(32);
            Property(e => e.AppName).HasMaxLength(256);
            Property(e => e.Name).IsRequired().HasMaxLength(256);
            Property(e => e.FriendlyName).HasMaxLength(256);
            Property(e => e.Description).IsMaxLength();
            Property(e => e.Value).IsMaxLength();
            Property(e => e.CreatedBy).HasMaxLength(256);
            Property(e => e.ModifiedBy).HasMaxLength(256);
            Property(e => e.RowVersion).IsConcurrencyToken().HasColumnType("Timestamp");
            Ignore(e => e.ObjectValue);
            Property(e => e.SourceId).HasMaxLength(32);
            Property(e => e.ValueTypeEnum).HasMaxLength(256);
            HasMany(e => e.ChildItems)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);
            Property(e => e.IsCompositeValue).IsRequired();
            Property(e => e.IsDeleted).IsRequired();
            Property(e => e.ItemsInited).IsRequired();
        }

    }
}
