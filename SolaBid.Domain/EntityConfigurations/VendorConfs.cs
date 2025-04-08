using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class VendorConfs : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.Property(m => m.VendorCode)
                .HasMaxLength(70)
                .IsRequired(true);

            builder.Property(m => m.VendorName)
                .HasMaxLength(600)
                .IsRequired(true);

            builder.Property(m => m.Address1)
                .HasMaxLength(500)
                .IsRequired(true);
            builder.Property(m => m.Address2)
                .HasMaxLength(500);
            builder.Property(m => m.Address3)
                .HasMaxLength(500);

            builder.Property(m => m.Contact)
                .HasMaxLength(30);

            builder.Property(m => m.Phone)
                .HasMaxLength(250);
            builder.Property(m => m.ExternalEmail)
                .HasMaxLength(128);

            builder.Property(m => m.Country)
                .HasMaxLength(300)
                .IsRequired(true);

            builder.Property(m => m.TaxId)
               .HasMaxLength(100);

            builder.Property(m => m.CreatedBy)
               .HasMaxLength(600)
               .IsRequired(true);

            builder.Property(m => m.LastUpdateBy)
               .HasMaxLength(600)
               .IsRequired(true);
        }
    }
}
