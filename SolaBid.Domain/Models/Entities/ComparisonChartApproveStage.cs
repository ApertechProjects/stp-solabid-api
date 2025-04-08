using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartApproveStage
    {
        public int Id { get; set; }
        public int ComparisonChartId { get; set; }
        public int BidReferanceId { get; set; }
        public string BidReferanceItemRowPointer { get; set; }
        public int Stage { get; set; }
        public int ApproveStageDetailId { get; set; }
    }
}

