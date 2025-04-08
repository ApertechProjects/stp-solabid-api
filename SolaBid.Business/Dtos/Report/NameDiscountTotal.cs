using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.Report
{
    public class NameDiscountTotal
    {
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
