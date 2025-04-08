using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
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
    public class WarehouseController : ControllerBase
    {
        [HttpGet]
        public async Task<List<WarehouseDto>> GetWarehousesBySite(string siteId)
        {
            var warehouseResultDto = new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(siteId)).GetWarehousesBySite(await new SiteLogic().GetSiteName(siteId));
            return null;
        }
    }
}
