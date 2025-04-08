using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChart : BaseOptions
    {
        public ComparisonChart()
        {
            SingleSourceReasons = new HashSet<RELComparisonChartSingleSource>();
            ComparisonChartApprovalBaseInfos = new List<ComparisonChartApprovalBaseInfos>();
            ComparisonChartApproveStages = new HashSet<ComparisonChartApproveStage>();
            ComparisonChartHolds = new HashSet<ComparisonChartHold>();
        }
        public int Id { get; set; }
        public bool SingleSource { get; set; }
        public bool IsRealisedToSyteLine { get; set; }
        public string ComProcurementSpecialist { get; set; }
        public int Stage { get; set; }
        public bool Annex { get; set; }

        public string WonnedLineTotalUSD { get; set; }
        public string WonnedLineTotalAZN { get; set; }

        public string ResponsiblePerson { get; set; }
        public string ResponsiblePersonId { get; set; }
        public DateTime? ResponsibilityDate { get; set; }
        //Relation Properties
        public BIDComparison BIDComparison { get; set; }
        public int BIDComparisonId { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
        public int ApproveStatusId { get; set; }
        public ApproveStageMain ApproveStage { get; set; }
        public int ApproveStageId { get; set; }
        public ComparisonChartReject ComparisonChartReject { get; set; }
        public ICollection<ComparisonChartHold> ComparisonChartHolds { get; set; }
        public List<ComparisonChartApprovalBaseInfos> ComparisonChartApprovalBaseInfos { get; set; }
        public ICollection<RELComparisonChartSingleSource> SingleSourceReasons { get; set; }
        public ICollection<ComparisonChartApproveStage> ComparisonChartApproveStages { get; set; }
    }
}
