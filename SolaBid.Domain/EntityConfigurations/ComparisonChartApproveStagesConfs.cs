using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartApproveStagesConfs : IEntityTypeConfiguration<ComparisonChartApproveStage>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartApproveStage> builder)
        {
            builder.Property(m => m.BidReferanceItemRowPointer)
                .HasMaxLength(512);
        }
    }

}
