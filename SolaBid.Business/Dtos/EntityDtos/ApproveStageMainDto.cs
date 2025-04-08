using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class ApproveStageMainDto :BaseOptions
    {
        public ApproveStageMainDto()
        {
            ApproveStageDetails = new HashSet<ApproveStageDetailDto>();
        }
        public int Id { get; set; }
        public string ApproveStageName { get; set; }
        public string Description { get; set; }

        public ICollection<ApproveStageDetailDto> ApproveStageDetails { get; set; }
    }
}
