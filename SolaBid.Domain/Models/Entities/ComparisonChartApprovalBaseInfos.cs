using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartApprovalBaseInfos
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int ComparisonChartId { get; set; }
        public ApproveStageDetail ApproveStageDetail { get; set; }
        public int ApproveStageDetailId { get; set; }
        public string ApprovedUserId { get; set; }
        public AppUser ApprovedUser { get; set; }
        public DateTime ApproveDate { get; set; }
        public string TotalApprovedAmount { get; set; }
    }
}
