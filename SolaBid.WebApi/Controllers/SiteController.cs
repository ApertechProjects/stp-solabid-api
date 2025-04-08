using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SiteController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<KeyValueTextBoxingDto>> GetSites()
        {
            var sites = await new SiteLogic().GetSites();
            return sites;
        }
        [HttpGet]
        public async Task<List<SiteWarehouseDto>> GetSiteWarehouses()
        {
            return await new SiteLogic().GetSiteWarehouses();
        }

        [HttpGet("{groupId}")]
        public async Task<List<SiteWarehouseDto>> GetGroupSiteWarehousesWithAll(string groupId)
        {
            var result = await new SiteLogic().GetGroupSiteWarehousesWithAll(groupId);
            return result;
        }
    }
}
