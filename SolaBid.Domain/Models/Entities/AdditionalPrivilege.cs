using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class AdditionalPrivilege
    {
        public AdditionalPrivilege()
        {
            GroupAdditionalPrivileges = new HashSet<GroupAdditionalPrivilege>();
        }
        public int Id { get; set; }
        public string PrivilegeName { get; set; }
        public string Description { get; set; }

        public ICollection<GroupAdditionalPrivilege> GroupAdditionalPrivileges  { get; set; }
    }
}
