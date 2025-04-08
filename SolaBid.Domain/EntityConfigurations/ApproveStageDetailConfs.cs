using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ApproveStageDetailConfs : IEntityTypeConfiguration<ApproveStageDetail>
    {
        public void Configure(EntityTypeBuilder<ApproveStageDetail> builder)
        {
            builder.Property(m => m.ApproveStageDetailName)
                .HasMaxLength(128)
                .IsRequired(true);
        }
    }
}
