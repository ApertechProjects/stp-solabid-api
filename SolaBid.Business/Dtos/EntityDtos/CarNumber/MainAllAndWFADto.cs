using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public class MainAllAndWFADto
    {
        public List<MainAllDto> All { get; set; }
        public List<MainWaitingForApproveDto> WaitingForApprove { get; set; }
    }
}
