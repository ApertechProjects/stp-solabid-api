using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class ValConvertorDto
    {
        public decimal _AZN { get; set; }
        public decimal _USD { get; set; }

        public string AZN { get { return _AZN == 0 ? "0" : _AZN.ToString("#.##"); } }
        public string USD { get { return _USD == 0 ? "0" : _USD.ToString("#.##"); } }
    }
}
