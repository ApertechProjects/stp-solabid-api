using SolaBid.Domain.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ApproveRoleDto
    {
        public int Id { get; set; }
        [Required]
        public string ApproveRoleName { get; set; }
        public bool IsSelected { get; set; }
        public string Description { get; set; }
        public ICollection<ApproveRoleApproveStageDetail> ApproveRoleApproveStageDetails { get; set; }
        public ICollection<GroupApproveRole> GroupApproveRoles { get; set; }
    }
}
