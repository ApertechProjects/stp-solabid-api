namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ApproveRoleApproveStageDetailDto
    {
        public int Id { get; set; }

        public int ApproveRoleId { get; set; }
        public int ApproveStageDetailId { get; set; }

        public ApproveRoleDto ApproveRole { get; set; }
        public ApproveStageDetailDto ApproveStageDetail { get; set; }

        public int AmountFrom { get; set; }
        public int AmountTo { get; set; }
    }
}
