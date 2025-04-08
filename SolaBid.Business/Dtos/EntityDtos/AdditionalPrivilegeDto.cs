using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class AdditionalPrivilegeDto
    {
        public AdditionalPrivilegeDto()
        {
            GroupAdditionalPrivileges = new HashSet<GroupAdditionalPrivilegeDto>();
        }
        public int Id { get; set; }
        public string PrivilegeName { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public ICollection<GroupAdditionalPrivilegeDto> GroupAdditionalPrivileges { get; set; }
    }
}
