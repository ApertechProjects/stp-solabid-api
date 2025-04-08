using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class CarNumberVendorListDto
    {
        public System.String VendorCode { get; set; }
        public System.String VendorName { get; set; }
        public System.String VendorClass { get; set; }
        public System.Boolean VendorExclude { get; set; }
    }
    public partial class CarNumberVendorListDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}
