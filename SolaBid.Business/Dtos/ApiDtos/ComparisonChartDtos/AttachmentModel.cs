using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class AttachmentModel
    {
        public AttachmentModel()
        {
            Attachments = new List<BIDAttachmentDto>();
        }
        public string BidComparisonNumber { get; set; }
        public List<BIDAttachmentDto> Attachments { get; set; }
    }
}
