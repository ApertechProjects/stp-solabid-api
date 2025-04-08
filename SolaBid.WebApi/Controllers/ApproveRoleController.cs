using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ApproveRoleController : ControllerBase
    {
        [HttpGet]
        public async Task<List<ApproveRoleDto>> GetApproveRoles()
        {
            return await new ApproveRoleLogic().GetApproveRoles();
        }

        [HttpGet]
        public async Task<List<KeyValueTextBoxingDto>> GetApproveStageMainsWithKeyValue()
        {
            return await new ApproveRoleLogic().GetApproveRolesWithKeyValue();
        }

        [HttpGet("{groupId}")]
        public async Task<List<ApproveRoleDto>> GetGroupApproveRolesWithAll(string groupId)
        {
            return await new ApproveRoleLogic().GetGroupApproveRolesWithAll(groupId);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> Create(ApproveRoleDto approveRoleDto)
        {
            return await new ApproveRoleLogic().Create(approveRoleDto, ModelState);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> Edit(ApproveRoleDto approveRoleDto)
        {
            return await new ApproveRoleLogic().Edit(approveRoleDto, ModelState);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> Delete(IntegerSingleId approveRoleId)
        {
            return await new ApproveRoleLogic().Delete(approveRoleId.Id);
        }
    }
}
