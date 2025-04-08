using SolaBid.Business.Dtos.ApiDtos;
using System;
using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BIDReferanceDto
    {
        public BIDReferanceDto()
        {
            Attachments = new HashSet<BIDAttachmentDto>();
            RequestItems = new HashSet<RELComparisonRequestItemDto>();
            BidItemsForEdit = new List<ReqItemCombinationDto>();
            SendedAttachments = new List<SendedAttachmentDto>();
        }
        //DtoDatas Start
        public string RequestNumber { get; set; }
        public List<KeyValueTextBoxingDto> Statuses { get; set; }
        public List<KeyValueTextBoxingDto> ApprovalStatuses { get; set; }
        public List<KeyValueTextBoxingDto> Wons { get; set; }
        public List<KeyValueTextBoxingDto> DeliveryTerms { get; set; }
        public List<KeyValueTextBoxingDto> PaymentTerms { get; set; }
        public List<KeyValueTextBoxingDto> Currencies { get; set; }
        public List<KeyValueTextBoxingDto> VendorKeyValues { get; set; }
        public List<KeyValueTextBoxingDto> DiscountTypes { get; set; }
        public List<BIDRequestItemDto> BIDRequestItems { get; set; }
        public List<VendorDto> Vendors { get; set; }
        public List<RELComparisonRequestItemDto> SelectedRequestItems { get; set; }
        public List<SendedAttachmentDto> SendedAttachments { get; set; }
        public List<ReqItemCombinationDto> BidItemsForEdit { get; set; }
        public string ComparisonDeadlineFormatted { get; set; }
        public string ComparisonDateFormatted { get; set; }
        public string RequestDateFormatted { get { return RequestDate.ToString("dd.MM.yyyy"); } set { } }
        public string EntryDateFormatted { get; set; }
        public bool ComparisanDataIsEditable { get; set; }
        public string CEOApproveDateFormatted { get; set; }

        //DtoDatas End
        public int Id { get; set; }
        public string Requester { get; set; }
        public string BIDNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ComparisonDate { get; set; }
        public DateTime ComparisonDeadline { get; set; }
        public string DeliveryTime { get; set; }
        public string Destination { get; set; }
        public string OR { get; set; }
        public decimal BudgetBalance { get; set; }
        public decimal ExpectedDelivery { get; set; }
        public string PONumber { get; set; }
        public string ComparisonChartPrepared { get; set; }
        public string ProjectWarehouse { get; set; }
        public string DeliveryTerm { get; set; }
        public string DeliveryDescription { get; set; }
        public string PayementTerm { get; set; }
        public string PaymentDescription { get; set; }
        public string Currency { get; set; }
        public decimal DiscountPrice { get; set; } = 0;
        public decimal DiscountValue { get; set; } = 0;
        public decimal DiscountedTotalPrice { get; set; } = 0;
        public decimal TotalQuantity { get; set; }
        public decimal LinePercentValue { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public decimal CurrentCurrTotal { get; set; }
        public decimal AZNTotal { get; set; }
        public decimal USDTotal { get; set; }

        //Relations
        public SiteDto Site { get; set; }
        public int SiteId { get; set; }
        public string VendorName { get; set; }
        public VendorDto Vendor { get; set; }
        public int VendorId { get; set; }
        public StatusDto Status { get; set; }
        public AppUserDto User { get; set; }
        public string UserId { get; set; }
        public int StatusId { get; set; }
        public ApproveStatusDto ApproveStatus { get; set; }
        public int ApproveStatusId { get; set; }
        public WonStatusDto WonStatus { get; set; }
        public int WonStatusId { get; set; }
        public BIDComparisonDto BIDComparison { get; set; }
        public int BIDComparisonId { get; set; }
        public DiscountTypeDto DiscountType { get; set; }
        public int DiscountTypeId { get; set; } = 1;
        public ICollection<RELComparisonRequestItemDto> RequestItems { get; set; }
        public ICollection<BIDAttachmentDto> Attachments { get; set; }

    }
}
