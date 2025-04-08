using SolaBid.Business.Dtos.ApiDtos;
using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class RELComparisonRequestItemDto
    {
        public RELComparisonRequestItemDto()
        {
            UOMItems = new List<KeyValueTextBoxingDto>();
        }
        public bool IsSelected { get; set; }
        public int Id { get; set; }
        public int BidLine { get; set; }
        public string RowPointer { get; set; }
        public string LineDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal LinePercentValue { get; set; }
        public decimal LineTotalDiscount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UniqueCurrencyUnitPrice { get; set; }
        public int BIDReferanceId { get; set; }
        public string PUOMFullText { get; set; }
        public string PUOMValue { get; set; }
        public decimal Conv { get; set; }
        public decimal ConvQuantity { get; set; }
        public decimal ConvUnitPrice { get; set; }
        public BIDReferanceDto BIDReferance { get; set; }
        public int OrderNum { get; set; } = 0;
        public List<KeyValueTextBoxingDto> UOMItems { get; set; }

    }
}
