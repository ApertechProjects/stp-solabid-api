using SolaBid.Business.Dtos.ApiDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BIDRequestItemDto
    {
        public BIDRequestItemDto()
        {
            UOMItems = new List<KeyValueTextBoxingForUOMDto>();
        }
        public Int16 RequestLine { get; set; }
        public int BidLine { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string UOM { get; set; }
        public decimal Budget { get; set; }
        public string RowPointer { get; set; }
        public string RefType { get; set; }
        public List<KeyValueTextBoxingForUOMDto> UOMItems { get; set; }
    }
}
