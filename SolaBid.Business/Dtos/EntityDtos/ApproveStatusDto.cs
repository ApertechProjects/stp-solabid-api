using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ApproveStatusDto
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public ICollection<BIDReferanceDto> BIDReferances { get; set; }
    }
}
