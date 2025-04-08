using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public class AttachmentLoadDto
    {
        public int AttachmentId { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }

    }
}
