using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class BIDReferanceInformation
    {
        public BIDReferanceInformation()
        {
            BIDInformationItems = new List<BIDInformationItems>();
            ApproveStageDetails = new List<ApproveStageDetailDto>();
            ApproveStageDetailsWithPercent = new List<ApproveStageDetailPercentage>();
        }
        public int BIDReferanceId { get; set; }
        public bool IsBestPrice { get; set; }
        public string ApproveStatusName { get; set; }
        public string BIDReferanceNumber { get; set; }
        public string EntryDateFormatted { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");
        public string VendorName { get; set; }
        public string DeliveryTerms { get; set; }
        public bool  IsWon { get; set; }
        public string DeliveryTime { get; set; }
        public string PaymentTerms { get; set; }
        public string Currency { get; set; }
        public string BudgetBalance { get; set; }
        public string ExpectedDelivery { get; set; }
        public string TotalAmount { get; set; }
        public string TotalAZN { get; set; }
        public string TotalUSD { get; set; }
        public string DiscountPrice { get; set; }
        public string DiscountValue { get; set; }
        public string DiscountedTotalPrice { get; set; }
        public bool HasAttachment { get; set; }
        public List<BIDInformationItems> BIDInformationItems { get; set; }
        public List<ApproveStageDetailDto> ApproveStageDetails { get; set; }
        public List<ApproveStageDetailPercentage> ApproveStageDetailsWithPercent { get; set; }
    }
}
