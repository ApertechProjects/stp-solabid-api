using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class DiscountTypeDto
    {
        public DiscountTypeDto()
        {
            BIDReferances = new HashSet<BIDReferanceDto>();
        }
        public int Id { get; set; }
        public string DiscountTypeName { get; set; }
        public ICollection<BIDReferanceDto> BIDReferances { get; set; }
    }
}
