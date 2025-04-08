using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class AdditionalPrivilegeController : ControllerBase
    {
        [HttpGet]
        public async Task<List<AdditionalPrivilegeDto>> GetAllAdditionalPrivileges() => await new AdditionalPrivilegeLogic().GetAdditionalPrivileges();

        [HttpGet("{groupId}")]
        public async Task<List<AdditionalPrivilegeDto>> GetAllAdditionalPrivilegesWithAll(string groupId) => await new AdditionalPrivilegeLogic().GetGroupAdditionalPrivilegesWithAll(groupId);
    }
}
