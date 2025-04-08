using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class BIDRequest
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public ICollection<BIDComparison> BIDComparisons { get; set; }
    }
}
