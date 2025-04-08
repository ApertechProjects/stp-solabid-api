using System.Collections.Generic;

namespace SolaBid.Domain.Models.Entities
{
    public class Status
    {
        public Status()
        {
            BIDReferances = new HashSet<BIDReferance>();   
        }
        public int Id { get; set; }
        public string StatusName { get; set; }
        public ICollection<BIDReferance>  BIDReferances { get; set; }
    }
}
