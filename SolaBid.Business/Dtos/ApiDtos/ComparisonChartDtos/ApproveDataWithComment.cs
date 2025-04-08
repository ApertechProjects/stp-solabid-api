using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
   public class ApproveDataWithComment
    {
        public ApproveDataWithComment()
        {
            ApproveDataModels = new List<ApproveDatasModel>();
        }
        public string Comment { get; set; }
        public string ApprovalStageTotalPrice { get; set; }
        public List<ApproveDatasModel> ApproveDataModels { get; set; }
    }
}
