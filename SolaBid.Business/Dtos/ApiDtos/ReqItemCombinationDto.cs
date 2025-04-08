using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class ReqItemCombinationDto
    {

        public ReqItemCombinationDto()
        {
            UOMItems = new List<KeyValueTextBoxingForUOMDto>();
        }
        public int SequenceNumber { get; set; }
        public int Id { get; set; }
        public string RowPointer { get; set; }
        public string LineDescription { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal LinePercentValue { get; set; } = 0;
        public decimal LineTotalDiscount { get; set; }
        public decimal Discount { get; set; } = 0;
        public int BidLine { get; set; }
        public decimal RequestQuantity { get; set; }
        public string UOM { get; set; }
        public decimal Budget { get; set; }
        public string RefType { get; set; }
        public string PUOMFullText { get; set; }
        public string PUOMValue { get; set; }
        public decimal Conv { get; set; }
        public decimal ConvQuantity { get; set; }
        public decimal ConvUnitPrice { get; set; }
        public List<KeyValueTextBoxingForUOMDto> UOMItems { get; set; }
    }
}
