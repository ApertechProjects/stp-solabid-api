using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class SubMenuPrivilegesDto
    {
        public int ParentId { get; set; }
        public int SubMenuId { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Export { get; set; }
    }
}
