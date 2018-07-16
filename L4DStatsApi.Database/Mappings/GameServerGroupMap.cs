using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class GameServerGroupMap : IEntityTypeConfiguration<GameServerGroupModel>
    {
        public void Configure(EntityTypeBuilder<GameServerGroupModel> builder)
        {
            builder.HasKey(gsg => gsg.Id);
            builder.HasAlternateKey(gsg => gsg.PrivateKey);
            builder.HasAlternateKey(gsg => gsg.PublicKey);

            builder.Property(gsg => gsg.Id).HasDefaultValue();
            builder.Property(gsg => gsg.PrivateKey).HasDefaultValue();
            builder.Property(gsg => gsg.PublicKey).HasDefaultValue();
            builder.Property(gsg => gsg.EmailAddress).IsRequired();
            builder.Property(gsg => gsg.IsActive).HasDefaultValue();
            builder.Property(gsg => gsg.IsValid).HasDefaultValue();

            builder.HasMany(gsg => gsg.GameServers)
                .WithOne(gs => gs.Group)
                .HasForeignKey(gs => gs.GroupId);
        }
    }
}
