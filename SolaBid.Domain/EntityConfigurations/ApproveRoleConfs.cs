using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ApproveRoleConfs : IEntityTypeConfiguration<ApproveRole>
    {
        public void Configure(EntityTypeBuilder<ApproveRole> builder)
        {
            builder.Property(m => m.ApproveRoleName)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Property(m => m.Description)
                .HasMaxLength(128);
        }
    }

}
