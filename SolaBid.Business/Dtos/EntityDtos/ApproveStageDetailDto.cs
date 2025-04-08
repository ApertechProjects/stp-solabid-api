using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ApproveStageDetailDto
    {
        public ApproveStageDetailDto()
        {
            ApproveRoleApproveStageDetails = new List<ApproveRoleApproveStageDetailDto>();
        }
        public int Id { get; set; }
        public string ApproveStageDetailName { get; set; }
        public int Sequence { get; set; }
        public int ApproveStageMainId { get; set; }
        public ApproveStageMainDto ApproveStageMain { get; set; }
        public ICollection<ApproveRoleApproveStageDetailDto> ApproveRoleApproveStageDetails { get; set; }

        //DTO Datas
        public string Comment { get; set; }
        public string NameSurname { get; set; }
        public string  ApproveDate { get; set; }
        public string Signature { get; set; }
        public string SignaturePath { get; set; }
        public string ApprovedUserId { get; set; }
        public string TotalApprovedAmount { get; set; }
        public int BidReferanceId { get; set; }
        public bool IsSelected { get; set; }
    }
}
