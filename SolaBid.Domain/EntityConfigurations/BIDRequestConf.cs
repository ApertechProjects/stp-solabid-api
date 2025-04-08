using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.EntityConfigurations
{
    public class BIDRequestConf : IEntityTypeConfiguration<BIDRequest>
    {
        public void Configure(EntityTypeBuilder<BIDRequest> builder)
        {
            builder.Property(m => m.RequestNumber)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }
}
