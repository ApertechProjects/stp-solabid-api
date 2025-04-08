using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class ApproveStageController : ControllerBase
    {
        [HttpGet]
        public async Task<List<ApproveStageMainDto>> GetApproveStageMains()
        {
            return await new ApproveStageLogic().GetApproveStageMains();
        }

        [HttpGet("{approveStageMainId}")]
        public async Task<List<ApproveStageMainFormDto>> GetApproveStageMainWithDetails(int approveStageMainId)
        {
            return await new ApproveStageLogic().GetApproveStageMainWithDetails(approveStageMainId);
        }
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> Save(ApproveStageMainFormDto approveStage)
        {
            var modifiedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await new ApproveStageLogic().SaveApproveStage(approveStage, ModelState, modifiedUserId);
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> Edit(ApproveStageMainFormDto approveStage)
        {
            var modifiedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await new ApproveStageLogic().EditApproveStage(approveStage, ModelState, modifiedUserId);
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> Delete(IntegerSingleId approveStageMainId)
        {
            return await new ApproveStageLogic().DeleteApproveStageMain(approveStageMainId.Id);
        }

    }
}
