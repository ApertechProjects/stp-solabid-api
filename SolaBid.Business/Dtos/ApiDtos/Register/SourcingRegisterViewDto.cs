using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class SourcingRegisterViewDto
    {
        public List<SourceRegisterDto> DailyRegisterSourcing { get; set; }
        public SourceRegisterDto EmptyRow { get; set; }
        public bool IsChecked { get; }
    }
}
