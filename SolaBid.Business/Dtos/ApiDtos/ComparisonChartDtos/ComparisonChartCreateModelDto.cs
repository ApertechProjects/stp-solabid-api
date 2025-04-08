using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ComparisonChartCreateModelDto
    {
        public ComparisonChartCreateModelDto()
        {
            ApprovedRowDatas = new List<ApprovedRowData>();
            SingleSourceDatas = new List<int>();
        }

        public int BidComparisonId { get; set; }
        public int ApproveStatusId { get; set; }
        public int StatusId { get; set; }
        public int ApproveStageId { get; set; }
        public bool SingleSource { get; set; }
        public string Comment{ get; set; }
        public int Stage { get; set; }
        public bool OneTimePo { get; set; }
        public bool Annex { get; set; }
        public List<ApprovedRowData> ApprovedRowDatas { get; set; }
        public List<int> SingleSourceDatas { get; set; }
    }
}
