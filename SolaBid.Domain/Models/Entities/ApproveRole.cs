using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class ApproveRole
    {
        public ApproveRole()
        {
            GroupApproveRoles = new HashSet<GroupApproveRole>();
            ApproveRoleApproveStageDetails = new List<ApproveRoleApproveStageDetail>();
        }
        public int Id { get; set; }
        public string ApproveRoleName { get; set; }
        public string Description { get; set; }
        public ICollection<ApproveRoleApproveStageDetail>  ApproveRoleApproveStageDetails { get; set; }
        public ICollection<GroupApproveRole> GroupApproveRoles { get; set; }
    }
}
