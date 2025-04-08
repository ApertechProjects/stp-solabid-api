using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public partial class BIDReferanceConf : IEntityTypeConfiguration<BIDReferance>
    {
        public void Configure(EntityTypeBuilder<BIDReferance> builder)
        {
            builder.Property(m => m.BIDNumber)
                .IsRequired(true)
                .HasMaxLength(100);
            builder.Property(m => m.ComparisonChartPrepared)
              .IsRequired(true)
              .HasMaxLength(512);
            builder.Property(m => m.Currency)
              .IsRequired(true)
              .HasMaxLength(50);
            builder.Property(m => m.DeliveryDescription)
                  .IsRequired(true)
                  .HasMaxLength(512);
            builder.Property(m => m.DeliveryTerm)
              .IsRequired(true)
              .HasMaxLength(100);
            builder.Property(m => m.Destination)
              .IsRequired(true)
              .HasMaxLength(100);
            builder.Property(m => m.PayementTerm)
              .IsRequired(true)
              .HasMaxLength(100);
            builder.Property(m => m.PONumber)
              .IsRequired(false)
              .HasMaxLength(100);
            builder.Property(m => m.CEOApproveDateFormatted)
              .IsRequired(false)
              .HasMaxLength(20);
            builder.Property(m => m.OR)
              .IsRequired(false)
              .HasMaxLength(20);
            builder.Property(m => m.PaymentDescription)
              .IsRequired(true)
              .HasMaxLength(512);
            builder.Property(m => m.ProjectWarehouse)
            .IsRequired(true)
            .HasMaxLength(512);
            builder.Property(m => m.Requester)
            .IsRequired(true)
            .HasMaxLength(100);
            builder.Property(m => m.DeliveryTime)
            .IsRequired(true)
            .HasMaxLength(500);
        }
    }
}
