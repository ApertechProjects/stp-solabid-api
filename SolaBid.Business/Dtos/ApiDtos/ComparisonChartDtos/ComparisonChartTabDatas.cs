using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ComparisonChartTabDatas
    {
        public ComparisonChartTabDatas()
        {
            AllComparisons = new List<ComparisonChartMainListData>();
            WaitingForApproval = new List<ComparisonChartMainListData>();
            Drafts = new List<ComparisonChartMainListData>();
            Rejected = new List<ComparisonChartMainListData>();
            Holded = new List<ComparisonChartMainListData>();
            MyCharts = new List<ComparisonChartMainListData>();
            NotRealised = new List<ComparisonChartMainListData>();
        }
        public List<ComparisonChartMainListData> AllComparisons { get; set; }
        public List<ComparisonChartMainListData> WaitingForApproval { get; set; }
        public List<ComparisonChartMainListData> Drafts { get; set; }
        public List<ComparisonChartMainListData> Rejected { get; set; }
        public List<ComparisonChartMainListData> Holded { get; set; }
        public List<ComparisonChartMainListData> MyCharts { get; set; }
        public List<ComparisonChartMainListData> NotRealised { get; set; }
    }


    public class ComparisonChartTabDatas2
    {
        public ComparisonChartTabDatas2()
        {
            AllComparisons = new List<ComparisonChartMainListData2>();
            WaitingForApproval = new List<ComparisonChartMainListData2>();
            Drafts = new List<ComparisonChartMainListData2>();
            Rejected = new List<ComparisonChartMainListData2>();
            Holded = new List<ComparisonChartMainListData2>();
            MyCharts = new List<ComparisonChartMainListData2>();
            NotRealised = new List<ComparisonChartMainListData2>();
        }
        public List<ComparisonChartMainListData2> AllComparisons { get; set; }
        public List<ComparisonChartMainListData2> WaitingForApproval { get; set; }
        public List<ComparisonChartMainListData2> Drafts { get; set; }
        public List<ComparisonChartMainListData2> Rejected { get; set; }
        public List<ComparisonChartMainListData2> Holded { get; set; }
        public List<ComparisonChartMainListData2> MyCharts { get; set; }
        public List<ComparisonChartMainListData2> NotRealised { get; set; }
    }
}
