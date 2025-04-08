using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Dtos.ApiDtos.Register
{
    public class RequestComparisonCreateDto
    {
        [Required]
        public int DailyRegisterId { get; set; }
        [Required]
        public string RequestNumber { get; set; }
        [Required]
        public string ComparisonNumber { get; set; }
    }
}
