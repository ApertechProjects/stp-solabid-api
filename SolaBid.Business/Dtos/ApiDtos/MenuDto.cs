using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class MenuDto
    {
        public MenuDto()
        {
            SubMenus = new List<SubMenuParentMenuDto>();
        }
        public int ParentMenuId { get; set; }
        public string ParentMenuName { get; set; }
        public int OrderNumber { get; set; }
        public List<SubMenuParentMenuDto> SubMenus { get; set; }
    }
}
