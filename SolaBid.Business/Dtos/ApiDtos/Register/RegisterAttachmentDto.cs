using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class RegisterAttachmentDto
    {
        public int DailyRegisterAttachmentId { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public string FolderName { get; set; }
    }
}
