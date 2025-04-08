using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
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
    public class MenuController : ControllerBase
    {
        [HttpGet]
        public async Task<List<MenuDto>> GetMenus()
        {
            return await new MenuLogic().GetMenus();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<ParentMenuDto>> GetMenuForTest()
        {
            return await new MenuLogic().GetUserMenusForTest();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<SubMenuForDXGroupGridDto>> GetMenuForGroupTest()
        {
            return await new MenuLogic().GetUserMenusForGroupTest();
        }


        [HttpGet("{groupId}")]
        public async Task<List<MenuDto>> GetGroupMenusWithAll(string groupId)
        {
            return await new MenuLogic().GetGroupMenusWithAll(groupId);
        }

        [HttpGet("{baseUrl}/{userId}")]
        public async Task<PrivilegeCheckerDto> CheckPrivilege(string baseUrl, string userId)
        {
            return await new MenuLogic().GetPrivileges(baseUrl, userId);
        }

        [HttpGet("{userId}")]
        public async Task<List<ParentMenuDto>> GetUserMenus(string userId)
        {
            return await new MenuLogic().GetUserMenus(userId);
        }

    }
}
