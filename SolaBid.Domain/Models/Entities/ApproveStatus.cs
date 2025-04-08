using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class ApproveStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public ICollection<BIDReferance>  BIDReferances { get; set; }
    }
}
