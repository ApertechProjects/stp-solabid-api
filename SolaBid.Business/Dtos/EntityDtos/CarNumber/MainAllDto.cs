using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class MainAllDto
    {
        public System.Int32 CarNumberMainId { get; set; }
        public System.Int32 Attachments { get; set; }
        public System.Int64 RowNum { get; set; }
        public System.String VendorCode { get; set; }
        public System.String VendorName { get; set; }
        public System.String OrderNumber { get; set; }
        public System.String CarNumber { get; set; }
        public System.String EQaimeNo { get; set; }
        public System.String OrderStatus { get; set; }
        public System.Int32 Status { get; set; }
        public System.String StatusName { get; set; }
        public System.String InventoryStatus { get; set; }
        public System.Int32 OrderReceivedStatus { get; set; }
        public System.String Comment { get; set; }
        public System.Int32 Stage { get; set; }
        public System.Boolean VendorExclude { get; set; }
        public System.String VendorClass { get; set; }
        public System.String DriverName { get; set; }
        public System.Boolean Entry { get; set; }
        public System.Boolean Exit { get; set; }
    }
    public partial class MainAllDto
    {
        public MainAllDto()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public List<PoMainRelationDto> OrdersObjList
        {
            get
            {
                if (!string.IsNullOrEmpty(OrderNumber))
                {
                    var orderNumbers = OrderNumber.TrimStart(';').TrimEnd(';').Split(';');
                    var resultData = new List<PoMainRelationDto>();
                    foreach (var po in orderNumbers)
                    {
                        resultData.Add(new PoMainRelationDto { Id = Guid.NewGuid(), OrderMainId = Id, PoNumber = po });
                    }
                    return resultData;
                }
                else return new List<PoMainRelationDto>();
            }
        }
    }
}
