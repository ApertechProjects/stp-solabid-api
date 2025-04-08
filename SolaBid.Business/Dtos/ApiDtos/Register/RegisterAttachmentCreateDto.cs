using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class RegisterAttachmentCreateDto
    {
        public int DailyRegisterAttachmentId { get; set; }
        public int DailyRegisterId { get; set; }
        public string RequestNumber { get; set; }
        public string ComparisonNumber { get; set; }
        public string OrderNumber { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }

    }
}
