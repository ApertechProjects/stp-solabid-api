using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BIDComparisonDto
    {
        public BIDComparisonDto()
        {
            BIDReferances = new HashSet<BIDReferanceDto>();
        }
        public int Id { get; set; }
        public int ReviseNumber { get; set; }
        public string ComparisonNumber { get; set; }
        public string ComparisonChartPrepared { get; set; }
        public int ProjectSiteId { get; set; }
        public BIDRequestDto BIDRequest { get; set; }
        public int BIDRequestId { get; set; }
        public ICollection<BIDReferanceDto> BIDReferances { get; set; }
        public ComparisonChartDto  ComparisonChart { get; set; }
    }
}
