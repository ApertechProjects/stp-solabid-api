using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartChatUserLastViewConf : IEntityTypeConfiguration<ComparisonChartChatUserLastView>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartChatUserLastView> builder)
        {
            builder.Property(m => m.ComparisonNumber)
                .HasMaxLength(100);
        }

    }
}
