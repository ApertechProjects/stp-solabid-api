namespace SolaBid.Domain.Models.Entities
{
    public class GroupAdditionalPrivilege
    {
        public int AdditionalPrivilegeId { get; set; }
        public string AppRoleId { get; set; } //GroupId

        public AppRole AppRole { get; set; } //Group
        public AdditionalPrivilege AdditionalPrivilege { get; set; }
    }
}
