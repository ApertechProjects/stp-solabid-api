using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartHoldConfs : IEntityTypeConfiguration<ComparisonChartHold>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartHold> builder)
        {
            builder.Property(m => m.HoldReason)
                .HasMaxLength(1024);

        }
    }

}
