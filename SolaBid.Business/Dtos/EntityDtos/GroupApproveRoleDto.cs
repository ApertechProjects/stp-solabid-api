namespace SolaBid.Business.Dtos.EntityDtos
{
    public class GroupApproveRoleDto
    {
        public string AppRoleId { get; set; } //GroupId
        public int ApproveRoleId { get; set; }
        public AppRoleDto AppRole { get; set; } //Group
        public ApproveRoleDto ApproveRole { get; set; }
    }
}
