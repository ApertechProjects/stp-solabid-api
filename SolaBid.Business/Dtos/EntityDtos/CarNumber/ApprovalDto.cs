using System;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class ApprovalDto
    {
        public System.Int32 CarNumberApprovalId { get; set; }
        public System.Int32 CarNumberMainId { get; set; }
        public System.Int32 Stage { get; set; }
        public System.String ApproveStatus { get; set; }
        public System.String ApprovedUserId { get; set; }
        public System.String UserName { get; set; }
        public System.DateTime ApproveDate { get; set; }
    }

    public partial class ApprovalDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}
