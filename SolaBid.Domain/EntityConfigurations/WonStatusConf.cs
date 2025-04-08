using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{

    public class WonStatusConf : IEntityTypeConfiguration<WonStatus>
    {
        public void Configure(EntityTypeBuilder<WonStatus> builder)
        {
            builder.Property(m => m.StatusName)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }

}
