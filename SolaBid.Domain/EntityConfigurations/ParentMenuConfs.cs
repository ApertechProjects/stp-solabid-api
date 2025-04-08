using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ParentMenuConfs : IEntityTypeConfiguration<ParentMenu>
    {
        public void Configure(EntityTypeBuilder<ParentMenu> builder)
        {
            builder.Property(m => m.Icon)
                .HasMaxLength(128)
                .IsRequired(true);

            builder.Property(m => m.ParentMenuName)
           .HasMaxLength(128)
           .IsRequired(true);
        }
    }
}
