using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class AppRoleDto
    {
        public AppRoleDto()
        {
            GroupSiteWarehouses = new HashSet<GroupSiteWarehouseDto>();
            GroupBuyers = new HashSet<GroupBuyerDto>();
            GroupAdditionalPrivileges = new HashSet<GroupAdditionalPrivilegeDto>();
        }
        public string Id { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(128)]
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public ICollection<GroupSiteWarehouseDto> GroupSiteWarehouses { get; set; }
        public ICollection<GroupBuyerDto> GroupBuyers { get; set; }
        public ICollection<GroupAdditionalPrivilegeDto> GroupAdditionalPrivileges { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
