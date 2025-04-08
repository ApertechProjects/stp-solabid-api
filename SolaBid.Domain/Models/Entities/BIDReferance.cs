using System;
using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class BIDReferance
    {
        public int Id { get; set; }
        public string Requester { get; set; }
        public string BIDNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ComparisonDate { get; set; }
        public DateTime ComparisonDeadline { get; set; }
        public decimal BudgetBalance { get; set; }
        public decimal ExpectedDelivery { get; set; }
        public string DeliveryTime { get; set; }
        public string Destination { get; set; }
        public string OR { get; set; }
        public string PONumber { get; set; }
        public string ComparisonChartPrepared { get; set; }
        public string ProjectWarehouse { get; set; }
        public string DeliveryTerm { get; set; }
        public string DeliveryDescription { get; set; }
        public string PayementTerm { get; set; }
        public string PaymentDescription { get; set; }
        public string Currency { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal DiscountPrice { get; set; } = 0;
        public decimal DiscountValue { get; set; } = 0;
        public decimal DiscountedTotalPrice { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public decimal CurrentCurrTotal { get; set; }
        public decimal AZNTotal { get; set; }
        public decimal USDTotal { get; set; }
        public string CEOApproveDateFormatted { get; set; }
        //Relations
        public Site Site { get; set; }
        public int SiteId { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public Vendor Vendor { get; set; }
        public int VendorId { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
        public int ApproveStatusId { get; set; }
        public WonStatus WonStatus { get; set; }
        public int WonStatusId { get; set; }
        public BIDComparison BIDComparison { get; set; }
        public int BIDComparisonId { get; set; }
        public DiscountType DiscountType { get; set; }
        public int DiscountTypeId { get; set; }
        public ICollection<RELComparisonRequestItem> RequestItems { get; set; }
        public ICollection<BIDAttachment> Atachments { get; set; }
    }
}
