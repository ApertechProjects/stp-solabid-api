using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ComparisonChartDto : BaseOptions
    {
        public ComparisonChartDto()
        {
            SingleSourceReasons = new HashSet<RELComparisonChartSingleSourceDto>();
            ComparisonChartApprovalBaseInfos = new List<ComparisonChartApprovalBaseInfosDto>();
            ComparisonChartApproveStages = new HashSet<ComparisonChartApproveStageDto>();
            ComparisonChartHolds = new HashSet<ComparisonChartHoldDto>();
        }

        public int Id { get; set; }
        public bool SingleSource { get; set; }
        public bool IsRealisedToSyteLine { get; set; }
        public string ComProcurementSpecialist { get; set; }
        public int Stage { get; set; }
        public bool Annex { get; set; }

        public string ResponsiblePersonId { get; set; }
        public string ResponsiblePerson { get; set; }
        public DateTime? ResponsibilityDate { get; set; }

        //Relation Properties
        public BIDComparisonDto BIDComparison { get; set; }
        public int BIDComparisonId { get; set; }
        public StatusDto Status { get; set; }
        public int StatusId { get; set; }
        public ApproveStatusDto ApproveStatus { get; set; }
        public int ApproveStatusId { get; set; }
        public ApproveStageMainDto ApproveStage { get; set; }
        public int ApproveStageId { get; set; }

        //Relation Properties
        public ICollection<ComparisonChartHoldDto> ComparisonChartHolds { get; set; }
        public ComparisonChartRejectDto ComparisonChartReject { get; set; }
        public List<ComparisonChartApprovalBaseInfosDto> ComparisonChartApprovalBaseInfos { get; set; }
        public ICollection<RELComparisonChartSingleSourceDto> SingleSourceReasons { get; set; }
        public ICollection<ComparisonChartApproveStageDto> ComparisonChartApproveStages { get; set; }
    }
}
