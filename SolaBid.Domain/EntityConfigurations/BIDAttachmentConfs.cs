using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.EntityConfigurations
{
    public class BIDAttachmentConfs : IEntityTypeConfiguration<BIDAttachment>
    {
        public void Configure(EntityTypeBuilder<BIDAttachment> builder)
        {
            builder.Property(m => m.FileName)
                .HasMaxLength(500); 
            builder.Property(m => m.FileBaseType)
         .HasMaxLength(500);
        }
    }
}
