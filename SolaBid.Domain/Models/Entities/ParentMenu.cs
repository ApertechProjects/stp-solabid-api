using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class ParentMenu
    {
        public int Id { get; set; }
        public string ParentMenuName { get; set; }
        public string Icon { get; set; }
        public int OrderNumber { get; set; }
        public ICollection<SubMenu> SubMenus { get; set; }
    }
}
