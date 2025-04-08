using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartRejectConfs : IEntityTypeConfiguration<ComparisonChartReject>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartReject> builder)
        {
            builder.Property(m => m.RejectReason)
                .HasMaxLength(1024);
        }
    }
}
