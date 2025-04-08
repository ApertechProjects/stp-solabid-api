using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{

    public class RELComparisonRequestItemConf : IEntityTypeConfiguration<RELComparisonRequestItem>
    {
        public void Configure(EntityTypeBuilder<RELComparisonRequestItem> builder)
        {
            builder.Property(m => m.LineDescription)
                .HasMaxLength(512);

            builder.Property(m => m.RowPointer)
              .IsRequired(true)
              .HasMaxLength(100);


            builder.Property(m => m.PUOMFullText)
            .HasMaxLength(100);

            builder.Property(m => m.PUOMValue)
            .HasMaxLength(3);


            builder.Property(m => m.Conv)
            .HasPrecision(18, 4);
        }
    }
}

