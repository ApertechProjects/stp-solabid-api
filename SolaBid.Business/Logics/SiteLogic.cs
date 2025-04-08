using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class SiteLogic
    {
        public async Task<List<KeyValueTextBoxingDto>> GetSites()
        {
            var siteListDto = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var siteEntities = await context.Sites.FirstAsync();
                List<Site> sites = new List<Site>() { siteEntities };
                siteListDto = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(sites);
            }

            return siteListDto;
        }

        public async Task<string> GetSiteDatabase(string siteId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var siteEntity = await context.Sites.FindAsync(int.Parse(siteId));
                return siteEntity.SiteDatabase;
            }
        }

        public async Task<string> GetSiteName(string siteId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var siteEntity = await context.Sites.FindAsync(int.Parse(siteId));
                return siteEntity.SiteName;
            }
        }

        public async Task<SiteDto> GetSite(string siteId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                return TransactionConfig.Mapper.Map<SiteDto>(await context.Sites.FindAsync(int.Parse(siteId)));
            }
        }

        public async Task<List<SiteWarehouseDto>> GetSiteWarehouses()
        {
            var siteWarehouseDtoList = new List<SiteWarehouseDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var Sites = await context.Sites.ToListAsync();
                foreach (var site in Sites)
                {
                    var siteDto = TransactionConfig.Mapper.Map<SiteDto>(site);
                    var warehouseDtoList =
                        new SiteLineDbLogic(site.SiteDatabase).GetWarehousesBySite(site.SiteName, site.Id);
                    siteWarehouseDtoList.Add(new SiteWarehouseDto
                    {
                        Site = siteDto,
                        SiteWarehouses = warehouseDtoList
                    });
                }
            }

            return siteWarehouseDtoList;
        }

        public async Task<List<SiteWarehouseDto>> GetGroupSiteWarehousesWithAll(string groupId)
        {
            var groupSiteWarehousesWithAll = await GetSiteWarehouses();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupSiteWarehouses =
                    await context.GroupSiteWarehouses.Where(m => m.AppRoleId == groupId).ToListAsync();
                if (groupSiteWarehouses.Count > 0)
                {
                    foreach (var siteWarehouse in groupSiteWarehousesWithAll)
                    {
                        var siteId = siteWarehouse.Site.Id;
                        foreach (var warehouse in siteWarehouse.SiteWarehouses)
                        {
                            if (groupSiteWarehouses.Any(m =>
                                    m.SiteId == siteId && m.WarehouseCode == warehouse.Warehouse))
                            {
                                warehouse.IsSelected = true;
                            }
                        }
                    }
                }
            }

            return groupSiteWarehousesWithAll;
        }

        public async Task<List<GroupSiteWarehouse>> GetGroupSiteWarehouses(string groupId, int siteId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupSiteWarehouses = await context.GroupSiteWarehouses
                    .Where(m => m.AppRoleId == groupId && m.SiteId == siteId)
                    .ToListAsync();
                return groupSiteWarehouses;
            }
        }
    }
}