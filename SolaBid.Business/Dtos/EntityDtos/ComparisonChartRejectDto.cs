using System;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ComparisonChartRejectDto
    {
        public int Id { get; set; }
        public string RejectReason { get; set; }
        public DateTime RejectDate { get; set; }
        public ApproveStageDetailDto RejectedStageDetail { get; set; }
        public int RejectedStageDetailId { get; set; }
        public string UserId { get; set; }
        public AppUserDto User { get; set; }
        public ComparisonChartDto ComparisonChart { get; set; }
        public int ComparsionChartId { get; set; }
    }
}
