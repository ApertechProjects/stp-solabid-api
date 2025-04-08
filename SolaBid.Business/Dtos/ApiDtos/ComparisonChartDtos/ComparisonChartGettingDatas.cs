using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Domain.Models.Entities;
using System.Collections.Generic;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ComparisonChartGettingDatas
    {
        public ComparisonChartGettingDatas()
        {
            RequestInformations = new List<RequestInformationDto>();
            BidReferanceInformations = new List<BIDReferanceInformation>();
            ApprovalStageDetails = new List<ApproveStageDetailDto>();
        }
        //Dto Datas
        public List<KeyValueTextBoxingDto> Statuses { get; set; }
        public List<KeyValueTextBoxingDto> ApprovalStatuses { get; set; }
        public List<KeyValueTextBoxingDto> ApprovalStages { get; set; }
        public List<ApproveStageDetailDto> ApprovalStageDetails { get; set; }
        public bool  IsDraftable { get; set; }
        public bool ChatIsAviable { get; set; }
        //Dto DatasEnd

        public bool CanRevise { get; set; }
        public string ComparisonChartId { get; set; }
        public string ApprovalStageId { get; set; }
        public string Entity { get; set; }
        public bool OneTimePo { get; set; }
        public bool Annex { get; set; }
        public string LastSentMessageTo { get; set; }
        public string LastUpdateMessageDate { get; set; }
        public string Requester { get; set; }
        public string RequestNo { get; set; }
        public string Destination { get; set; }
        public string OR { get; set; }
        public string Buyer { get; set; }
        public string ComparisonNumber { get; set; }
        public string WarehouseName { get; set; }
        public string ComparisonDate { get; set; }
        public string ComparisonDeadline { get; set; }
        public string WonTotalAZN { get; set; }
        public string WonTotalUSD { get; set; }
        public string WonLastPurchasedPriceUSD { get; set; }
        public string WonnerVendorAndLines { get; set; }
        public string SingleSourceReason { get; set; }
        public int Stage { get; set; }
        public string ApproveStageName { get; set; }
        public int ApproveStageDetailId { get; set; }
        public int StatusId { get; set; }
        public string ComProcurementSpecialist { get; set; }
        public int ApproveStatusId { get; set; }
        public List<RequestInformationDto> RequestInformations { get; set; }
        public List<BIDReferanceInformation> BidReferanceInformations { get; set; }
       
    }
}
