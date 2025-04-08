namespace SolaBid.Domain.Models.Entities
{
    public class VendorAttachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileBaseType { get; set; }
        public string FileName { get; set; }
        public int VendorId { get; set; }

    }
}
