using System;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ComparisonChartApprovalBaseInfosDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int ComparisonChartId { get; set; }
        public ApproveStageDetailDto ApproveStageDetail { get; set; }
        public int ApproveStageDetailId { get; set; }
        public string ApprovedUserId { get; set; }
        public AppUserDto ApprovedUser { get; set; }
        public DateTime ApproveDate { get; set; }
        public string TotalApprovedAmount { get; set; }
    }
}
