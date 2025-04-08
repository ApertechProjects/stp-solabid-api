using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos
{
   public class UsdAznDiscount
    {
        public decimal TotalUsd { get; set; } = 0;
        public decimal TotalAzn { get; set; } = 0;
        public decimal Discount{ get; set; } = 0;

    }
}
