using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class VendorEditDto
    {
        public VendorEditDto()
        {
            SendedAttachments = new List<SendedAttachmentDto>();
        }
        public List<KeyValueTextBoxingDto> Countries { get; set; }
        public List<KeyValueTextBoxingDto> Currencies { get; set; }
        public List<KeyValueTextBoxingDto> TaxCodes { get; set; }
        public List<KeyValueTextBoxingDto> PaymentTerms { get; set; }
        public List<KeyValueTextBoxingDto> DeliveryTerms { get; set; }
        public List<KeyValueTextBoxingDto> BankCodes { get; set; }
        public List<SendedAttachmentDto> SendedAttachments { get; set; }
        public int Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public bool VendorBlackList { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string ExternalEmail { get; set; }
        public string Country { get; set; }
        public string TaxCode { get; set; }
        public string TaxId { get; set; }
        public string DeliveryTerm { get; set; }
        public string PaymentTerm { get; set; }
        public string Currency { get; set; }
        public string BankCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FormattedCreateDate { get { return CreatedDate.ToString("dd.MM.yyyy"); } }
        public DateTime EditDate { get; set; }
        public string FormattedEditDate { get { return EditDate.ToString("dd.MM.yyyy"); } }
        public string LastUpdateBy { get; set; }
        public bool HasSiteLine { get; set; }
        public bool userCanEditVendorWithSiteLine { get; set; }
    }
}
