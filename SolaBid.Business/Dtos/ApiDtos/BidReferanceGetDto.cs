using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class BidReferanceGetDto:BIDReferance
    {
        public string ApproveStatusName { get; set; }
        public string StatusName { get; set; }
        public string VendorName { get; set; }
    }
}
