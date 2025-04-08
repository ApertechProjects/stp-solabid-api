namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ComparisonChartApproveStageDto
    {
        public int Id { get; set; }
        public int ComparisonChartId { get; set; }
        public int BidReferanceId { get; set; }
        public string BidReferanceItemRowPointer { get; set; }
        public int Stage { get; set; }
        public int ApproveStageDetailId { get; set; }
    }
}
