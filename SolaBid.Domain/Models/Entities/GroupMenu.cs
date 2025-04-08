namespace SolaBid.Domain.Models.Entities
{
    public class GroupMenu
    {
        public int Id { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Export { get; set; }
        public string AppRoleId { get; set; }  //Group
        public AppRole AppRole { get; set; } //Group
        public int SubMenuId { get; set; }
        public SubMenu SubMenu { get; set; }
    }
}
