using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ApproveStatusConf : IEntityTypeConfiguration<ApproveStatus>
    {
        public void Configure(EntityTypeBuilder<ApproveStatus> builder)
        {
            builder.Property(m => m.StatusName)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }
}
