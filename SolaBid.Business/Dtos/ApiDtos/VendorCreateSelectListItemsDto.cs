using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class VendorCreateSelectListItemsDto
    {
        public List<KeyValueTextBoxingDto> Countries { get; set; }
        public List<KeyValueTextBoxingDto> Currencies { get; set; }
        public List<KeyValueTextBoxingDto> TaxCodes { get; set; }
        public List<KeyValueTextBoxingDto> PaymentTerms { get; set; }
        public List<KeyValueTextBoxingDto> DeliveryTerms { get; set; }

    }
}
