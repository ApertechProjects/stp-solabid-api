using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ComparisonChartConf : IEntityTypeConfiguration<ComparisonChart>
    {
        public void Configure(EntityTypeBuilder<ComparisonChart> builder)
        {
            builder.Property(m => m.ComProcurementSpecialist)
                .HasMaxLength(800);
            builder.Property(m => m.WonnedLineTotalAZN)
         .HasMaxLength(80);
            builder.Property(m => m.WonnedLineTotalUSD)
         .HasMaxLength(80);

            builder.Property(m => m.ResponsiblePerson)
                .HasMaxLength(100);
            builder.Property(m => m.ResponsiblePersonId)
            .HasMaxLength(100);
            builder.Property(m => m.IsRealisedToSyteLine)
           .HasDefaultValue(true);
        }

    }
}
