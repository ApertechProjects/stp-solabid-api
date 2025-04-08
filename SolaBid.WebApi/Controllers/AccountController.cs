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
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using SolaBid.Business.Dtos.SingleObjs;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Dependency
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _webHosEnvironment;
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHosEnvironment = webHostEnvironment;
        }
        #endregion
        [HttpPost]
        [Route("Login")]
        public async Task<LoginResult> Login([FromBody] LoginDto login)
        {
     
            var result = await new AccountLogic().Login(login, _signInManager, ModelState, Request.Host.ToString());
            if (result.IsAuthorized)
            {
                login.UserId = result.UserId;
                var token = TokenHelper.TokenHelper.GenerateToken(login);
                result.JWTToken = token;
            }
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [Route("Register")]
        public async Task<ApiResult> RegistrationUser([FromBody] AppUserDto registredUser)
        {
            var result = await new AccountLogic().RegistrationUser(registredUser, _userManager, _webHosEnvironment.WebRootPath, ModelState);
            return result;
        }

        [HttpPost]
        [Route("Restore")]
        public async Task<ApiResult> Restore(StringSingleId id)
        {
            var result = await new AccountLogic().Restore(id,_webHosEnvironment.WebRootPath);
            return result;
        }

        [HttpGet("CheckRestoreToken/{tokenData}")]
        public async Task<bool> CheckRestoreToken(string tokenData)
        {
            bool result = await new AccountLogic().CheckRestoreToken(tokenData);
            return result;
        }

        [HttpPost]
        [Route("restorepassword")]
        public async Task<ApiResult> RestorePassword(PasswordRestoreDto restoreDto )
        {
            var result = await new AccountLogic().RestorePassword(restoreDto.Password, restoreDto.Token, _userManager);
            return result;
        }


        [HttpGet]
        [Route("[action]")]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public ApiResult Test()
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //string userName = User.FindFirst(ClaimTypes.Name)?.Value;
            //string siteDatabase = User.FindFirst(ClaimTypes.System)?.Value;
            //var tes = new BIDLogic().GenereteBidNumber();
            return null;
        }

    }
}
