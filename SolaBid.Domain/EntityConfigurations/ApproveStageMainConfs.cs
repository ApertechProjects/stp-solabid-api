using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ApproveStageMainConfs : IEntityTypeConfiguration<ApproveStageMain>
    {
        public void Configure(EntityTypeBuilder<ApproveStageMain> builder)
        {
            builder.Property(m => m.ApproveStageName)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Property(m => m.Description)
               .HasMaxLength(512);
        }
    }
}
