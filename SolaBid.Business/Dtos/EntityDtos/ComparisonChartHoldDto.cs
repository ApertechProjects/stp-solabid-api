using System;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ComparisonChartHoldDto
    {
        public int Id { get; set; }
        public string HoldReason { get; set; }
        public DateTime HoldDate { get; set; }
        public ApproveStageDetailDto ApproveStageDetail { get; set; }
        public int ApproveStageDetailId { get; set; }
        public string UserId { get; set; }
        public AppUserDto User { get; set; }
        public ComparisonChartDto ComparisonChart { get; set; }
        public int ComparisonChartId { get; set; }
    }
}
