using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ApproveRoleApproveStageDetail
    {
        public int Id { get; set; }
        public int ApproveRoleId { get; set; }
        public int ApproveStageDetailId { get; set; }
         
        public ApproveRole  ApproveRole{ get; set; }
        public ApproveStageDetail  ApproveStageDetail { get; set; }

        public int AmountFrom { get; set; }
        public int AmountTo { get; set; }
    }
}
