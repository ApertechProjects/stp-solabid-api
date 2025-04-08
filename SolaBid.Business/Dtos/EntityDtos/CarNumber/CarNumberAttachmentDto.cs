using System;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class CarNumberAttachmentDto
    {

        public int CarNumberAttachmentId { get; set; }
        public int CarNumberMainId { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }

    public partial class CarNumberAttachmentDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}
