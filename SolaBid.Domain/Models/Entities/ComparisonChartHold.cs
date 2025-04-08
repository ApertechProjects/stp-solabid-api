using System;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartHold
    {
        public int Id { get; set; }
        public string HoldReason { get; set; }
        public DateTime HoldDate { get; set; }
        public ApproveStageDetail ApproveStageDetail { get; set; }
        public int ApproveStageDetailId { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public ComparisonChart ComparisonChart { get; set; }
        public int ComparisonChartId { get; set; }
    }

}
