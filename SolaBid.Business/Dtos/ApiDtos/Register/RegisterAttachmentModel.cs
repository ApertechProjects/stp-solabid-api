using System.Collections.Generic;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class RegisterAttachmentModel
    {
        public RegisterAttachmentModel()
        {
            Files = new List<RegisterAttachmentDto>();
        }
        public List<RegisterAttachmentDto> Files { get; set; }
        public string FolderName { get; set; }
    }
}
