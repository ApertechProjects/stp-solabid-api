using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ORDetailDto
    {

        public Guid Id { get { return Guid.NewGuid(); } }


        public System.String ORNumber { get; set; }
        public System.Int16 ORLine { get; set; }
        public System.String JobNumber { get; set; }
        public System.Int16 JobSuffix { get; set; }
        public System.String JobItem { get; set; }
        public System.String JobItemName { get; set; }
        public System.Decimal JobQty { get; set; }
        public System.Decimal JobIssuedQty { get; set; }
        public System.Decimal COSTinUSD { get; set; }
        public System.Decimal COSTinAZN { get; set; }
        public System.Decimal USDCOSTTOTAL { get; set; }
        public System.Decimal AZNCOSTTOTAL { get; set; }
        public System.String PONumber { get; set; }
        public System.Int16 PO_Line { get; set; }
        public System.String ItemCode { get; set; }
        public System.String ItemDescription { get; set; }
        public System.String VendorCode { get; set; }
        public System.String VendorName { get; set; }
        public System.String POCur { get; set; }
        public System.Decimal PONETAmount { get; set; }
        public System.Decimal POQty { get; set; }
        public System.Decimal POUnitCost { get; set; }
        public System.Decimal PaymentAmount { get; set; }
        public System.String PaymentCur { get; set; }
        public System.Decimal POReceivedQty { get; set; }
        public System.Decimal POReceivedTotal { get; set; }
        public System.Decimal OutstandingPOReceived { get; set; }

    }
}
