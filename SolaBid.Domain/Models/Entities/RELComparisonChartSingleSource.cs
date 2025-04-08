namespace SolaBid.Domain.Models.Entities
{
    public class RELComparisonChartSingleSource
    {
        public int Id { get; set; }
        public int ComparisonChartSingleSourceReasonId { get; set; }
        public ComparisonChartSingleSourceReason ComparisonChartSingleSourceReason { get; set; }
        public int ComparisonChartId { get; set; }
        public ComparisonChart ComparisonChart { get; set; }
    }
}
