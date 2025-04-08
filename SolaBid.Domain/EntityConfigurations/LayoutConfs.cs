using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class LayoutConfs : IEntityTypeConfiguration<Layout>
    {
        public void Configure(EntityTypeBuilder<Layout> builder)
        {
            builder.Property(m => m.Key)
                .HasMaxLength(300);
            builder.Property(m => m.UserId)
        .HasMaxLength(300);
        }
    }
}
