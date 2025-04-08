using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class AppRole : IdentityRole
    {
        public AppRole()
        {
            GroupSiteWarehouses = new HashSet<GroupSiteWarehouse>();
            GroupBuyers = new HashSet<GroupBuyer>();
            GroupAdditionalPrivileges = new HashSet<GroupAdditionalPrivilege>();
        }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<GroupSiteWarehouse> GroupSiteWarehouses { get; set; }
        public ICollection<GroupBuyer> GroupBuyers { get; set; }
        public ICollection<GroupApproveRole> GroupApproveRoles { get; set; }
        public ICollection<GroupAdditionalPrivilege> GroupAdditionalPrivileges { get; set; }

    }
}
