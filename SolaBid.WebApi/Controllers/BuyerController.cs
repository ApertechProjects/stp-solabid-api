using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BuyerController : ControllerBase
    {
        [HttpGet]
        public async Task<List<BuyerDto>> GetAllBuyers()
        {
            var buyers = new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetBuyers();
            return buyers;
        }
        [HttpGet("{groupId}")]
        public async Task<List<BuyerDto>> GetGroupBuyersWithAll(string groupId)
        {
            var buyers = await new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetGroupBuyersWithAll(groupId);
            return buyers;
        }

        [HttpGet("{userId}")]
        public async Task<List<BuyerDto>> GetUserBuyersWithAll(string userId)
        {
            var buyers = await new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetUserBuyersWithAll(userId);
            return buyers;
        }
    }
}
