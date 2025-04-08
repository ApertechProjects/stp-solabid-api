using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class BIDComparisonGetDto
    {
        public BIDComparisonGetDto()
        {
            BIDReferances = new HashSet<BIDReferance>();
        }
        public int Id { get; set; }
        public int ReviseNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string ComparisonNumber { get; set; }
        public string ComparisonChartPrepared { get; set; }
        public int ProjectSiteId { get; set; }
        public string RequestNumber { get; set; }
        public string ApproveStatusName { get; set; }
        public string StatusName { get; set; }
        public int BIDRequestId { get; set; }
        public ICollection<BIDReferance> BIDReferances { get; set; }
        public ComparisonChart ComparisonChart { get; set; }
    }
}
