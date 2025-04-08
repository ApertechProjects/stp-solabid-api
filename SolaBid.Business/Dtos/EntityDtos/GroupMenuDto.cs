namespace SolaBid.Business.Dtos.EntityDtos
{
    public class GroupMenuDto
    {
        public int Id { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Export { get; set; }
        public string AppRoleId { get; set; }  //Group
        public AppRoleDto AppRole { get; set; } //Group
        public int SubMenuId { get; set; }
        public SubMenuDto SubMenu { get; set; }
    }
}
