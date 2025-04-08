using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ApprovedRowData
    {
        [Required]
        public int BidReferanceId { get; set; }
        [Required]
        public string RowPointer { get; set; }
    }
}
