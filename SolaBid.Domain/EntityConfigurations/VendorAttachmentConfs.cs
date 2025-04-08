using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class VendorAttachmentConfs : IEntityTypeConfiguration<VendorAttachment>
    {
        public void Configure(EntityTypeBuilder<VendorAttachment> builder)
        {
            builder.Property(m => m.FileName)
                .HasMaxLength(500);
            builder.Property(m => m.FileBaseType)
         .HasMaxLength(500);
        }
    }
}
