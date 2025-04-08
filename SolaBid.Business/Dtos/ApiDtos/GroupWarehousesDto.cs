using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.ApiDtos
{
    public class GroupWarehousesDto
    {
        public string WarehouseId { get; set; } //WarehouseCode
        public string WarehouseName { get; set; }
        public int SiteId { get; set; }
        public string AppRoleId { get; set; }
    }
}
