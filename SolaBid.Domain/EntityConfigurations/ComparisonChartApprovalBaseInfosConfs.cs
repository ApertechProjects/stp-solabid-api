using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartApprovalBaseInfosConfs : IEntityTypeConfiguration<ComparisonChartApprovalBaseInfos>
    {
        public void Configure(EntityTypeBuilder<ComparisonChartApprovalBaseInfos> builder)
        {
            builder.Property(m => m.Comment)
                .HasMaxLength(1024);
               
        }
    }

}
