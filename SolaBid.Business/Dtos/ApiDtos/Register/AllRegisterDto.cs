using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class AllRegisterDto
    {
        public AllRegisterDto()
        {
            Id = Guid.NewGuid().ToString();
        }
        public int Sequence { get; set; }
        public string Id { get; init; }
        public System.String Buyer { get; set; }
        public System.String RequestNumber { get; set; }
        public System.String PRLines { get; set; }
        public System.String RequestType { get; set; }
        public System.String Requester { get; set; }
        public System.String Company { get; set; }
        public System.String Status { get; set; }
        public System.String RequestedFor { get; set; }
        public System.String ShortDescription { get; set; }
        public System.String RFQSuppliers { get; set; }
        public System.String ManualRFQSuppliers { get; set; }
        public System.String OrderNo { get; set; }
        public System.String ComparisonNumber { get; set; }
        public System.String Winner { get; set; }
        public System.String Currency { get; set; }
        public System.Int32 SourcingKPI { get; set; }
        public System.Int32 ProcurementKPI { get; set; }
        public System.String PriseInUSD { get; set; }
        public System.String Price { get; set; }
        public System.String OrderFirstApproval { get; set; }
        public System.String OrderSecondApproval { get; set; }
        public System.String DueDate { get; set; }
        public System.String DateOfRequest { get; set; }
        public System.String ReceivedDate { get; set; }
        public System.String RFQSentDate { get; set; }
        public System.String SourcingClosingDate { get; set; }
        public System.String ComparisonDate { get; set; }
        public System.String DeliveryNoteNumber { get; set; }
        public bool IsChecked { get; }
    }
}
