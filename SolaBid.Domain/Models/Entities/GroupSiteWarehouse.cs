using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class GroupSiteWarehouse
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public int SiteId { get; set; }
        public string AppRoleId { get; set; }
        public Site Site { get; set; }
        public AppRole AppRole { get; set; }
    }
}
