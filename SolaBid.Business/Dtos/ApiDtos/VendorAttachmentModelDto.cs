using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class VendorAttachmentModelDto
    {
        public VendorAttachmentModelDto()
        {
            VendorAttachments = new List<VendorAttachmentForMain>();
        }
        public int VendorId { get; set; }
        public List<VendorAttachmentForMain> VendorAttachments { get; set; }
    }
}
