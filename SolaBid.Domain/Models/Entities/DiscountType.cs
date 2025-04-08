using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class DiscountType
    {
        public DiscountType()
        {
            BIDReferances = new HashSet<BIDReferance>();
        }
        public int Id { get; set; }
        public string DiscountTypeName { get; set; }
        public ICollection<BIDReferance> BIDReferances { get; set; }
    }
}
