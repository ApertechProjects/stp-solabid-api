using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private IWebHostEnvironment _env;
        public UserController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public async Task<List<NewUsersDto>> GetNewUsers()
        {
            var result = await new UserLogic().GetNewUsers(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value));
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<NewUsersDto>> GetNewUsersForTest()
        {
            var result = await new UserLogic().GetNewUsersForTest();
            return result;
        }

        [HttpGet]
        public async Task<List<ApprovedUserListDto>> GetApprovedUsers()
        {
            var approvedUsersList = await new UserLogic().GetApprovedUsers();
            return approvedUsersList;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<ApprovedUserListDto>> GetApprovedUsersForTest()
        {
            var approvedUsersList = await new UserLogic().GetApprovedUsersForTest();
            return approvedUsersList;
        }

        [HttpGet]
        public async Task<List<UserListDto>> GetUsers()
        {
            var userListDto = await new UserLogic().GetUsers();
            return userListDto;
        }

        [HttpGet("{groupId}")]
        public async Task<List<ApprovedUserListDto>> GetGroupUsersWithAll(string groupId)
        {
            var result = await new UserLogic().GetGroupUsersWithAll(groupId);
            return result;
        }
        [HttpGet("{userId}")]
        public async Task<UserFormDataDto> GetUserFormDatas(string userId)
        {
            return await new UserLogic().GetUserFormDatas(userId, await new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetUserBuyersWithAll(userId));
        }

        [HttpGet("{key}")]
        public async Task<string> GetLayout(string key)
        {
            return await new UserLogic().GetLayout(User.FindFirst(ClaimTypes.NameIdentifier)?.Value,key);
        }

        [HttpGet]
        public async Task<UserFullnameBuyerNameDto> GetUserFullNameBuyerName() => await UserLogic.GetUserFullNameBuyerName(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet("{userId}")]
        public async Task<string> GetUserSignature(string userId)
        {
            return await UserLogic.GetUserSignature(userId);
        }
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> EditUser(EditUserDto editUser)
        {
            return await new UserLogic().EditUser(editUser, ModelState, _env.WebRootPath);
        }


        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> DeleteUser(StringSingleId userId)
        {
            return await new UserLogic().DeleteUser(userId.Id, _env.WebRootPath);
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> RejectUser(StringSingleId userId)
        {
            return await new UserLogic().RejectUser(userId.Id, _env.WebRootPath);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> ApproveNewUser(List<ApproveNewUserDto> approveNewUser)
        {
            var approveResult = await new UserLogic().ApproveNewUsersFromMain(approveNewUser, ModelState, _env.WebRootPath);
            return approveResult;
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> SetLayout(LayoutSaveDto layoutModel)
        {
            return await new UserLogic().SetLayout(User.FindFirst(ClaimTypes.NameIdentifier)?.Value,layoutModel);
        }
    }
}
