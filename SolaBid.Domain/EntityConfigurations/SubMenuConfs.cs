using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class SubMenuConfs : IEntityTypeConfiguration<SubMenu>
    {
        public void Configure(EntityTypeBuilder<SubMenu> builder)
        {
            builder.Property(m => m.Icon)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Property(m => m.SubLink)
             .HasMaxLength(128)
             .IsRequired(true);
            builder.Property(m => m.SubMenuName)
           .HasMaxLength(128)
           .IsRequired(true);
        }
    }
}
