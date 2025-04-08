using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class StatusDto
    {
        // public StatusDto()
        // {
        //     BIDReferances = new HashSet<BIDReferanceDto>();
        // }
        public int Id { get; set; }
        public string StatusName { get; set; }
        // public ICollection<BIDReferanceDto> BIDReferances { get; set; }
    }
}
