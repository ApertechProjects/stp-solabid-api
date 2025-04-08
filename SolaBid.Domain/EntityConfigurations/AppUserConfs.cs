using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.EntityConfigurations
{
    public class AppUserConfs : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(m => m.UserName)
              .IsRequired(true)
                .HasMaxLength(128);
            builder.Property(m => m.Email)
              .IsRequired(true)
                .HasMaxLength(128);
            builder.Property(m => m.FirstName)
              .IsRequired(true)
            .HasMaxLength(128);
            builder.Property(m => m.LastName)
              .IsRequired(true)
                .HasMaxLength(128);
            builder.Property(m => m.RegDate)
              .IsRequired(true);
            builder.Property(m => m.UserImage)
              .HasMaxLength(512);
            builder.Property(m => m.UserSignature)
              .HasMaxLength(512);
        }
    }
}
