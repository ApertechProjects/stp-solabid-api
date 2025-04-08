using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class PasswordRestoreDto
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
