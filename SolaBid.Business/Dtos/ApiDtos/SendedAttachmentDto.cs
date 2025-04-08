using System;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class SendedAttachmentDto
    {
        public string FileId { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

    }
}
