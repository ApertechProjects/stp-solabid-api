using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ApproveDatasModel
    {
        [Required]
        public string RowPointer { get; set; }
        [Required]
        public int ComparisonChartId { get; set; }
        [Required]
        public int BidReferanceId { get; set; }
        [Required]
        public int ApproveStageDetailId { get; set; }
        [Required]
        public int Stage { get; set; }
        public bool IsApproved { get; set; }

    }
}
