using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class ApproveStageMainFormDto
    {
        public ApproveStageMainFormDto()
        {
            ApproveStageDetails = new List<ApproveStageDetailFormDto>();
        }
        public int Id { get; set; }
        [Required,MaxLength(40)]
        public string ApproveStageMainName { get; set; }
        public string Description { get; set; }
        public List<int> DeletedAppStageDetailsList { get; set; }
        public List<int> DeletedAppStageDetailSubDetailsList { get; set; }
        public List<ApproveStageDetailFormDto> ApproveStageDetails { get; set; }
    }
}
