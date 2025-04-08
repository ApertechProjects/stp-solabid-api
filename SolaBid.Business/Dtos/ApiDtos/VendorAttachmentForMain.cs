using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class VendorAttachmentForMain
    {
        public string FileBase64 { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileId { get; set; }
        public string FileUrl { get; set; }
    }
}
