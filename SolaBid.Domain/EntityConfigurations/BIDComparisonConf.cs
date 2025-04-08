using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{

    public class BIDComparisonConf : IEntityTypeConfiguration<BIDComparison>
    {
        public void Configure(EntityTypeBuilder<BIDComparison> builder)
        {
            builder.Property(m => m.ComparisonNumber)
                .IsRequired(true)
                .HasMaxLength(100);
            builder.Property(m => m.ComparisonChartPrepared)
                .IsRequired(true)
                .HasMaxLength(200);
        }
    }
}

