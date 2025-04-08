using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
    public class BidRefWonnedLines
    {
        public BidRefWonnedLines()
        {
            BidItems = new List<RELComparisonRequestItem>();
        }
        public BIDReferance BIDReferance { get; set; }
        public List<RELComparisonRequestItem> BidItems { get; set; }
    }
}
