using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class BIDAttachmentDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public BIDReferanceDto BIDReferance { get; set; }
        public int BIDReferanceId { get; set; }
        public string TempBase64 { get; set; }
    }


}
