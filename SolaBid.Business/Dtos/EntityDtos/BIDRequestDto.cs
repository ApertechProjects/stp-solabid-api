using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BIDRequestDto
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public ICollection<BIDComparisonDto> BIDComparisons { get; set; }
    }
}
