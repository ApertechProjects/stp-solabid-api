namespace SolaBid.Business.Dtos.EntityDtos
{
    public class GroupAdditionalPrivilegeDto
    {
        public int AdditionalPrivilegeId { get; set; }
        public string AppRoleId { get; set; } //GroupId

        public AppRoleDto AppRole { get; set; } //Group
        public AdditionalPrivilegeDto AdditionalPrivilege { get; set; }
    }
}
