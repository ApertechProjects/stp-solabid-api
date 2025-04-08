using SolaBid.Business.Dtos.ApiDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class VendorDto
    {
        public VendorDto()
        {
            // BIDReferances = new HashSet<BIDReferanceDto>();
            VendorAttachments = new HashSet<VendorAttachmentDto>();
            SendedAttachments = new List<SendedAttachmentDto>();
        }
        //DTO Props
        public bool canDelete { get; set; }
        public bool canEditVendorWithSiteLine { get; set; }
        public bool hasAttachment { get; set; }
        public bool importToSyteline { get; set; }
        //DTO Props End
        public int Id { get; set; }
        public string VendorCode { get; set; }
        //[Required, MaxLength(60)]
        public string VendorName { get; set; }
        public bool VendorBlackList { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string ExternalEmail { get; set; }
        //[Required, MaxLength(30)]
        public string Country { get; set; }
        public string TaxCode { get; set; }
        public string TaxId { get; set; }
        //[Required]
        public string DeliveryTerm { get; set; }
        //[Required]
        public string PaymentTerm { get; set; }
        //[Required]
        public string Currency { get; set; }
        //[Required]
        public string BankCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FormattedCreateDate { get { return CreatedDate.ToString("dd/MM/yyyy"); } }
        public DateTime EditDate { get; set; }
        public string LastUpdateBy { get; set; }
        public bool HasSiteLine { get; set; }
        public List<SendedAttachmentDto> SendedAttachments { get; set; }
        public ICollection<VendorAttachmentDto> VendorAttachments { get; set; }
        // public ICollection<BIDReferanceDto> BIDReferances { get; set; }

    }
}
