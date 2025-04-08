namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class SourceRegisterCreateDto
    {
        public int DailyRegisterId { get; set; }
        public string Buyer { get; set; }
        public string RequestNumber { get; set; }
        public string RequestType { get; set; }
        public string Requester { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
        public string RequestedFor { get; set; }
        public string ShortDescription { get; set; }
        public string RFQSuppliers { get; set; }
        public string OrderNo { get; set; }
        public string Winner { get; set; }
        public string Currency { get; set; }
        public string PriseInUSD { get; set; }
        public string Price { get; set; }
        public string OrderFirstApproval { get; set; }
        public string OrderSecondApproval { get; set; }
        public string DateOfRequest { get; set; }
        public string RFQSentDate { get; set; }
        public string SourcingClosingDate { get; set; }
        public string ComparisonDate { get; set; }
        public System.String DeliveryNoteNumber { get; set; }
        public bool IsChecked { get; }
    }
}
