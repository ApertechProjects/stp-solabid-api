using System;
using System.Collections.Generic;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class BIDInformationItems
    {
        public BIDInformationItems()
        {
            ApprovalStageDetails = new List<ApproveDatasModel>();
        }
        public string RowPointer { get; set; }
        public decimal BidQuantity { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
        public bool IsBestUnitPrice { get; set; }
        public string Discount { get; set; }
        public string LineTotalDiscount { get; set; }
        public string BidComment { get; set; }
        public string DescriptionOfRequiredPurchase { get; set; }
        public string EntryDateFormatted { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");
        public string PUOMFullText { get; set; }
        public string PUOMValue { get; set; }
        public decimal Conv { get; set; }
        public decimal ConvQuantity { get; set; }
        public decimal ConvUnitPrice { get; set; }
        public List<ApproveDatasModel> ApprovalStageDetails { get; set; }

        //ForMobile
        public int ApproveStageDetailId { get; set; }
        public int BidReferanceId { get; set; }
        public int ComparisonChartId { get; set; }
        public int Stage { get; set; }

    }
}
