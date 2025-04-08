using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ChartGetMessagesDto
    {
        public string Message { get; set; }
        public string SendedDate { get; set; }
        public string FullName { get; set; }
        public bool IsRight { get; set; }
    }
}
