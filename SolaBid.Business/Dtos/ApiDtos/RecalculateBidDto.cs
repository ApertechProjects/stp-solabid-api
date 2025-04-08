using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class RecalculateBidDto
    {
        [Required]
        public int BidReferanceId { get; set; }
        public decimal TotalAZN { get; set; }
        public decimal TotalUSD { get; set; }
    }
}
