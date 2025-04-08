using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class SubMenuParentMenuDto
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Export { get; set; }
    }
}
