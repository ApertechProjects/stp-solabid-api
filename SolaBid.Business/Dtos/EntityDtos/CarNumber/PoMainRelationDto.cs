using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public class PoMainRelationDto
    {
        public Guid Id { get; set; }
        public Guid OrderMainId { get; set; }
        public string PoNumber { get; set; }
    }
}
