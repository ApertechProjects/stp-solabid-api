using System;
using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class BIDComparison
    {
        public BIDComparison()
        {
            BIDReferances = new HashSet<BIDReferance>();
        }
        public int Id { get; set; }
        public int ReviseNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string ComparisonNumber { get; set; }
        public string ComparisonChartPrepared { get; set; }
        public int ProjectSiteId { get; set; }
        public BIDRequest BIDRequest { get; set; }
        public int BIDRequestId { get; set; }
        public ICollection<BIDReferance> BIDReferances { get; set; }
        public ComparisonChart ComparisonChart { get; set; }
    }
}
