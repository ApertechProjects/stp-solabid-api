using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class ApproveStageDetailFormDto
    {
        public ApproveStageDetailFormDto()
        {
            ApproveRoles = new List<ApproveStageDetailSubDetailsDto>();
        }
        public int Id { get; set; }
        [Required, MaxLength(40)]
        public string ApproveStageDetailName { get; set; }
        [Required]
        public int ApproveStageDetailSequence { get; set; }
        public bool IsNew { get; set; }
        public List<ApproveStageDetailSubDetailsDto> ApproveRoles { get; set; }
    }
}
