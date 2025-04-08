using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolaBid.Domain.Models.Entities;

namespace SolaBid.Domain.EntityConfigurations
{
    public class GroupSiteWarehouseConfs : IEntityTypeConfiguration<GroupSiteWarehouse>
    {
        public void Configure(EntityTypeBuilder<GroupSiteWarehouse> builder)
        {
            builder.Property(m => m.WarehouseCode)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Property(m => m.WarehouseName)
                .HasMaxLength(512)
                .IsRequired(true);
        }
    }

}
