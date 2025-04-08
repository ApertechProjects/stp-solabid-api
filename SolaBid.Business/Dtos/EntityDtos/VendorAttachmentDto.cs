namespace SolaBid.Business.Dtos.EntityDtos
{
    public class VendorAttachmentDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public int VendorId { get; set; }
        public string TempBase64 { get; set; }

    }


}
