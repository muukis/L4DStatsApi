using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class WeaponMap : IEntityTypeConfiguration<WeaponModel>
    {
        public void Configure(EntityTypeBuilder<WeaponModel> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id).HasDefaultValue();
            builder.Property(w => w.Name).HasMaxLength(50).IsRequired();

            builder.HasOne(w => w.MatchPlayer)
                .WithMany(mp => mp.Weapons)
                .HasForeignKey(w => w.MatchPlayerId);

            builder.HasMany(w => w.WeaponTargets)
                .WithOne(wt => wt.Weapon)
                .HasForeignKey(wt => wt.WeaponId);
        }
    }
}
