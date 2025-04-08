using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ApproveStageMain : BaseOptions
    {
        public ApproveStageMain()
        {
            ApproveStageDetails = new HashSet<ApproveStageDetail>();
        }
        public int Id { get; set; }
        public string ApproveStageName { get; set; }
        public string Description { get; set; }

        //Relations
        public ICollection<ApproveStageDetail> ApproveStageDetails { get; set; }
    }
}
