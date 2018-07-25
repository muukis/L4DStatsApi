using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class WeaponTargetMap : IEntityTypeConfiguration<WeaponTargetModel>
    {
        public void Configure(EntityTypeBuilder<WeaponTargetModel> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(wt => wt.Id).HasDefaultValue();
            builder.Property(wt => wt.SteamId).HasMaxLength(50).IsRequired();
            builder.Property(wt => wt.Count).IsRequired();
            builder.Property(wt => wt.HeadshotCount).IsRequired();
            builder.Property(wt => wt.Type).IsRequired();

            builder.HasOne(wt => wt.Weapon)
                .WithMany(w => w.WeaponTargets)
                .HasForeignKey(wt => wt.WeaponId);
        }
    }
}
