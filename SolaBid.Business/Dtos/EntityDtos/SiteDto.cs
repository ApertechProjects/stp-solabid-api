using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class SiteDto
    {
        // public SiteDto()
        // {
        //     GroupSiteWarehouses = new HashSet<GroupSiteWarehouseDto>();
        // }

        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteDatabase { get; set; }

        // public ICollection<GroupSiteWarehouseDto> GroupSiteWarehouses { get; set; }
    }
}
