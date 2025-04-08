using System.Collections.Generic;

namespace SolaBid.Business.Dtos.EntityDtos.CarNumber
{
    public class InsertEditItemModelDto
    {
        public InsertEditItemModelDto()
        {
            NewAttachments = new();
            OrdersList = new();
        }
        public int CarNumberMainId { get; set; }
        public int Status { get; set; }
        public int Stage { get; set; }
        public string CarNumber { get; set; }
        public string VendorCode { get; set; }
        public string EQaimeNo { get; set; }
        public string Comment { get; set; }
        public string VendorClass { get; set; }
        public bool VendorExclude { get; set; }
        public string DriverName { get; set; }
        public bool Entry { get; set; }
        public bool Exit { get; set; }
        public List<CarNumberAttachmentDto> NewAttachments { get; set; }
        public List<string> OrdersList { get; set; }
    }
}
