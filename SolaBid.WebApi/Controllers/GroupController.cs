using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class GroupController : ControllerBase
    {
        #region Depends
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _webHosEnvironment;
        public GroupController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<AppRole> roleManager,
                                 IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHosEnvironment = webHostEnvironment;
        }
        #endregion

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> AddGroup(GroupDatasDto groupDatas)
        {
            return await new GroupLogic().AddGroup(groupDatas, _roleManager, ModelState);
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> EditGroup(GroupDatasDto groupDatas)
        {
            return await new GroupLogic().EditGroup(groupDatas, _roleManager, ModelState);
        }

        [HttpGet]
        public async Task<List<AppRoleDto>> GetGroups()
        {
            return await new GroupLogic().GetGroups();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<AppRoleDto>> GetGroupsForTest()
        {
            return await new GroupLogic().GetGroupsForTest();
        }

        [HttpGet("{groupId}")]
        public async Task<GroupFormDataDto> GetGroupFormData(string groupId)
        {
            return await new GroupLogic().GetGroupFormDatas(groupId);
        }

        [HttpPost]
        public async Task<ApiResult> DeleteGroup(GroupFormDataDto groupFormDataDto)
        {
            return await new GroupLogic().DeleteGroup(groupFormDataDto.Id);
        }

        [HttpGet("{userId}")]
        public async Task<List<AppRoleDto>> GetUserGroupsWithAll(string userId)
        {
            return await new GroupLogic().GetUserGroupsWithAll(userId);
        }

    }
}
