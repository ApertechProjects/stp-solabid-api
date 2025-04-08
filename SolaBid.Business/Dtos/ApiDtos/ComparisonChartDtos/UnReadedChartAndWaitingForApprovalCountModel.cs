using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class UnReadedChartAndWaitingForApprovalCountModel
    {
        public UnReadedChartAndWaitingForApprovalCountModel()
        {
            UnreadedCharts = new List<UnreadedChatModel>();
        }
        public int WaitingForApprovalCount { get; set; }
        public int UnreadedChartCount { get { return UnreadedCharts.Count; } }
        public List<UnreadedChatModel> UnreadedCharts { get; set; }
    }
}
