using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartSingleSourceReasonConf : IEntityTypeConfiguration<ComparisonChartSingleSourceReason>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartSingleSourceReason> builder)
        {
            builder.Property(m => m.SingleSourceReasonName)
                .IsRequired(true)
                .HasMaxLength(800);
        }
    }
}
