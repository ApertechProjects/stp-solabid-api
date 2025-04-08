using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
  public  class ComparisonChartRejectHoldModel
    {
        public int ComparisonChartId { get; set; }
        public int Stage { get; set; }
        public string ModalValue { get; set; } //RejectReason
    }
}
