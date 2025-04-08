namespace SolaBid.Domain.Models.Entities
{
    public class SubMenu
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string SubMenuName { get; set; }
        public string SubLink { get; set; }
        public int ParentMenuId { get; set; }
        public ParentMenu ParentMenu { get; set; }
    }
}
