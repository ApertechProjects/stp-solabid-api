using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class SiteConfs : IEntityTypeConfiguration<Site>
    {
        public void Configure(EntityTypeBuilder<Site> builder)
        {
            builder.Property(m => m.SiteName)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Property(m => m.SiteDatabase)
              .HasMaxLength(128)
              .IsRequired(true);
        }
    }
}
