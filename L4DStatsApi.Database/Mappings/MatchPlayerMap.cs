﻿using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L4DStatsApi.Mappings
{
    public class MatchPlayerMap : IEntityTypeConfiguration<MatchPlayerModel>
    {
        public void Configure(EntityTypeBuilder<MatchPlayerModel> builder)
        {
            builder.HasKey(mp => mp.Id);

            builder.Property(mp => mp.Id).HasDefaultValue();
            builder.Property(mp => mp.SteamId).HasMaxLength(50).IsRequired();
            builder.Property(mp => mp.Name).HasMaxLength(50).IsRequired();
            builder.Property(mp => mp.Kills).IsRequired();
            builder.Property(mp => mp.Deaths).IsRequired();

            builder.HasOne(mp => mp.Match)
                .WithMany(m => m.Players)
                .HasForeignKey(mp => mp.MatchId);
        }
    }
}
