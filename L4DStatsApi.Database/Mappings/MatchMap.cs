using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class MatchMap : IEntityTypeConfiguration<MatchModel>
    {
        public void Configure(EntityTypeBuilder<MatchModel> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasDefaultValue();
            builder.Property(m => m.GameServerId).IsRequired();
            builder.Property(m => m.GameName).HasMaxLength(64).IsRequired();
            builder.Property(m => m.MapName).HasMaxLength(100).IsRequired();
            builder.Property(m => m.Type).HasMaxLength(50).IsRequired();
            builder.Property(m => m.HasEnded).HasDefaultValue();
            builder.Property(m => m.SecondsPlayed).HasDefaultValue();
            builder.Property(m => m.StartTime).HasDefaultValue();
            builder.Property(m => m.LastActive).HasDefaultValue().ValueGeneratedOnAddOrUpdate();

            builder.HasOne(m => m.GameServer)
                .WithMany(gs => gs.Matches)
                .HasForeignKey(m => m.GameServerId);

            builder.HasMany(m => m.Players)
                .WithOne(mp => mp.Match)
                .HasForeignKey(mp => mp.MatchId);
        }
    }
}
