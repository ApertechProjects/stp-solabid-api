using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class ApproveStageDetail
    {
        public ApproveStageDetail()
        {
            ApproveRoleApproveStageDetails = new List<ApproveRoleApproveStageDetail>();
        }
        public int Id { get; set; }
        public string ApproveStageDetailName { get; set; }
        public int Sequence { get; set; }
        public int ApproveStageMainId { get; set; }
        public ApproveStageMain ApproveStageMain { get; set; }
        public ICollection<ApproveRoleApproveStageDetail> ApproveRoleApproveStageDetails { get; set; }

    }
}
