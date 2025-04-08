namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class RequestInformationDto
    {
        public string RowPointer { get; set; }
        public string DescriptionOfRequiredPurchase { get; set; }
        public int PRItemNo { get; set; }
        public string RequestQuantity { get; set; }
        public string UOM { get; set; }
        public string Budget { get; set; }
        public string LastPurchasedDate { get; set; }
        public string LastPurchasedPrice { get; set; }
        public int OrderNumber { get; set; }
    }
}
