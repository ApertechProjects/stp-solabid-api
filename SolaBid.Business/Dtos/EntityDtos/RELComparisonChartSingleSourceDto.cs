namespace SolaBid.Business.Dtos.EntityDtos
{
    public class RELComparisonChartSingleSourceDto
    {
        public int Id { get; set; }
        public int ComparisonChartSingleSourceReasonId { get; set; }
        public ComparisonChartSingleSourceReasonDto ComparisonChartSingleSourceReason { get; set; }
        public int ComparisonChartId { get; set; }
        public ComparisonChartDto ComparisonChart { get; set; }
    }
}
