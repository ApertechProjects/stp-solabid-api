using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class AdditionalPrivilegeConfs : IEntityTypeConfiguration<AdditionalPrivilege>
    {
        public void Configure(EntityTypeBuilder<AdditionalPrivilege> builder)
        {
            builder.Property(m => m.Description)
                .HasMaxLength(128);
            builder.Property(m => m.PrivilegeName)
                .HasMaxLength(128)
                  .IsRequired(true);
        }
    }
}
