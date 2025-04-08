using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class Vendor
    {
        public Vendor()
        {
            BIDReferances = new HashSet<BIDReferance>();
            VendorAttachments = new HashSet<VendorAttachment>();
        }
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
        public DateTime EditDate { get; set; }
        public string LastUpdateBy { get; set; }

        public ICollection<VendorAttachment> VendorAttachments { get; set; }
        public ICollection<BIDReferance> BIDReferances { get; set; }
    }
}
