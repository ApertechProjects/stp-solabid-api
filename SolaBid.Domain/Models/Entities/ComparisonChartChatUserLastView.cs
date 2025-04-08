using System;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartChatUserLastView
    {
        public int Id { get; set; }
        public int ComparisonChartId { get; set; }
        public string ComparisonNumber { get; set; }
        public string UserId { get; set; }
        public DateTime ViewDate { get; set; }
    }
}
