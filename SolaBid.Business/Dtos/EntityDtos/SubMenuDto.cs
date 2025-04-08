namespace SolaBid.Business.Dtos.EntityDtos
{
    public class SubMenuDto
    {
        public int Id { get; set; }
        public string SubMenuName { get; set; }
        public string Icon { get; set; }
        public string SubLink { get; set; }
        public int ParentMenuId { get; set; }
        public ParentMenuDto ParentMenu { get; set; }
    }
}
