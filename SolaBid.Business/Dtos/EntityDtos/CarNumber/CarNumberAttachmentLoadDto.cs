using System;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public partial class CarNumberAttachmentLoadDto
    {
        public System.Int32 CarNumberAttachmentId { get; set; }
        public System.Int32 CarNumberMainId { get; set; }
        public System.String FileName { get; set; }
        public System.Byte[] File { get; set; }
        public System.String FileBaseType { get; set; }
    }
    public partial class CarNumberAttachmentLoadDto
    {
        public Guid Id { get { return Guid.NewGuid(); } }
    }
}
