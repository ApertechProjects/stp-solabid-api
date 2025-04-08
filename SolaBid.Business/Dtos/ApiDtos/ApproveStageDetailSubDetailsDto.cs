using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
  public  class ApproveStageDetailSubDetailsDto
    {
        public int Id { get; set; }
        [Required]
        public int ApproveRoleMainId { get; set; }
        public int AmountFrom { get; set; }
        public int AmountTo { get; set; }
        public bool IsNew { get; set; }

    }
}
