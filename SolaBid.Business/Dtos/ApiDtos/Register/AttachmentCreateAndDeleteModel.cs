using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class AttachmentCreateAndDeleteModel
    {
        public AttachmentCreateAndDeleteModel()
        {
            UploadedFiles = new List<UploadedFileModel>();
            DeletedFiles = new List<RegisterAttachmentCreateDto>();
            DeletedFolders = new List<string>();
        }
        public List<UploadedFileModel> UploadedFiles { get; set; }
        public List<RegisterAttachmentCreateDto> DeletedFiles { get; set; }
        public List<string> DeletedFolders { get; set; }
        public string RequestNumber { get; set; }
        public string ComparisonNumber { get; set; }
        public string OrderNumber { get; set; }

    }
}
