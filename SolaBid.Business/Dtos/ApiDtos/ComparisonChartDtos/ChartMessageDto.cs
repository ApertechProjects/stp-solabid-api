using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class ChartMessageDto
    {
        [Required]
        public string Message { get; set; }
        public string AttachedBuyerEmail { get; set; }
        [Required]
        public int ComparisonChartId { get; set; }

    }
}
