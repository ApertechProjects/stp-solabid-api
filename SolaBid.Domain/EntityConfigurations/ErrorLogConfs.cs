using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class ErrorLogConfs : IEntityTypeConfiguration<ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.Property(m => m.ErrorDetail)
                .IsRequired(true);
            builder.Property(m => m.ErrorMessage)
              .IsRequired(true);
            builder.Property(m => m.ErrorStackTrace)
            .IsRequired(true);
        }
    }
}
