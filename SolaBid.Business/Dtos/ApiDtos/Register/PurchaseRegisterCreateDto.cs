namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class PurchaseRegisterCreateDto
    {
        public System.String RequestNumber { get; set; }
        public System.String ShortDescription { get; set; }
        public System.String ManualRFQSuppliers { get; set; }
        public System.String OrderNo { get; set; }
        public System.String ComparisonNumber { get; set; }
        public System.String RFQSentDate { get; set; }
        public System.String SourcingClosingDate { get; set; }
        public System.String DeliveryNoteNumber { get; set; }
        public bool IsChecked { get; }
    }
}
