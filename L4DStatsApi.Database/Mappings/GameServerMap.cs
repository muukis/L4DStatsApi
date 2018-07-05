using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class GameServerMap : IEntityTypeConfiguration<GameServerModel>
    {
        public void Configure(EntityTypeBuilder<GameServerModel> builder)
        {
            builder.HasKey(gs => gs.Id);
            builder.HasAlternateKey(gs => gs.Key);

            builder.Property(gs => gs.Id).HasDefaultValue();
            builder.Property(gs => gs.Key).HasDefaultValue();
            builder.Property(gs => gs.GroupId).IsRequired();
            builder.Property(gs => gs.IsActive).HasDefaultValue();
            builder.Property(gs => gs.IsValid).HasDefaultValue();
            builder.Property(gs => gs.Name).HasMaxLength(255).IsRequired();
            builder.Property(gs => gs.LastActive).HasDefaultValue();

            builder.HasMany(gs => gs.Matches)
                .WithOne(m => m.GameServer)
                .HasForeignKey(m => m.GameServerId);

            builder.HasOne(gs => gs.Group)
                .WithMany(gsg => gsg.GameServers)
                .HasForeignKey(gs => gs.GroupId);
        }
    }
}
