using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartReject
    {
        public int Id { get; set; }
        public string RejectReason { get; set; }
        public DateTime RejectDate { get; set; }
        public ApproveStageDetail RejectedStageDetail { get; set; }
        public int RejectedStageDetailId { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public ComparisonChart ComparisonChart { get; set; }
        public int ComparisonChartId { get; set; }
    }

}
