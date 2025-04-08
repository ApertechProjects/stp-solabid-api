using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Domain.Models.Entities
{
    public class ComparisonChartChat
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int OrderNumber { get; set; }
        public DateTime SendedDate { get; set; }
        public int ComparisonChartId { get; set; }
        public ComparisonChart  ComparisonChart { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
    }
}
