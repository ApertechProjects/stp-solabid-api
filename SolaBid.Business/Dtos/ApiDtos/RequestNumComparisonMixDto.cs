using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class RequestNumComparisonMixDto
    {
        public int Id { get; set; }
        public int ComparisonId { get; set; }
        public string ComparisonName { get; set; }
        public string RequestNumber { get; set; }
        public bool canGenerateBid { get; set; }
    }
}
