using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class GroupSiteWarehouseDto
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public int SiteId { get; set; }
        public string AppRoleId { get; set; }
        public SiteDto Site { get; set; }
        public AppRoleDto AppRole { get; set; }
    }
}
