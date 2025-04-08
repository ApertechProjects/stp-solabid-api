using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class CarNumberAdditionalPrivilegeDto
    {
        public bool CarSendToApprove { get; set; }
        public bool CarApprover1 { get; set; }
        public bool CarApprover2 { get; set; }
    }
}
