using SolaBid.Business.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class SiteWarehouseDto
    {
        public SiteDto Site { get; set; }
        public List<WarehouseDto> SiteWarehouses { get; set; }
    }
}
