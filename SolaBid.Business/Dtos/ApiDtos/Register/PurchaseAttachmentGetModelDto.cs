using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class PurchaseAttachmentGetModelDto
    {
        public string RequestNumber { get; set; }
        public string ComparisonNumber { get; set; }
        public string OrderNumber { get; set; }
    }
}
