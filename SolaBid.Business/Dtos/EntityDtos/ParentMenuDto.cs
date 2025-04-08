using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ParentMenuDto
    {
        public ParentMenuDto()
        {
            SubMenus = new HashSet<SubMenuDto>();
        }
        public int Id { get; set; }
        public string ParentMenuName { get; set; }
        public int OrderNumber { get; set; }
        public string Icon { get; set; }
        public ICollection<SubMenuDto> SubMenus { get; set; }
    }
}
