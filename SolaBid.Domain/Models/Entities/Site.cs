using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Domain.Models.Entities
{
    public class Site
    {
        public Site()
        {
            GroupSiteWarehouses = new HashSet<GroupSiteWarehouse>();
            BIDReferances = new HashSet<BIDReferance>();
        }
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteDatabase { get; set; }
        public ICollection<GroupSiteWarehouse> GroupSiteWarehouses { get; set; }
        public ICollection<BIDReferance> BIDReferances { get; set; }

    }
}
