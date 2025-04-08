using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos;
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

    public class ComparisonChartController : ControllerBase
    {
        public IWebHostEnvironment _env { get; }

        public ComparisonChartController(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ComparisonChartTabDatas2> GetComparisonChartMainDatas()
        {
            return await new ComparisonChartLogic().GetList(
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                     await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value),
                     int.Parse(User.FindFirst(ClaimTypes.System)?.Value)
                     );
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> WaitingForApprovals()
        {
            return await new ComparisonChartLogic().WaitingForApprovals(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> Drafts()
        {
            return await new ComparisonChartLogic().Drafts();
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> Held()
        {
            return await new ComparisonChartLogic().Held();
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> MyCharts()
        {
            return await new ComparisonChartLogic().MyCharts(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> All()
        {
            return await new ComparisonChartLogic().All();
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> NotRealised()
        {
            return await new ComparisonChartLogic().NotRealised();
        }

        [HttpGet]
        public async Task<List<ComparisonChartMainListData>> Rejected()
        {
            return await new ComparisonChartLogic().Rejected();
        }

        [HttpGet("{OR}")]
        public async Task<List<ORDetailDto>> OrDetail(string Or)
        {
            return await new ComparisonChartLogic().OrDetail(
                 await new SiteLogic().GetSiteName(User.FindFirst(ClaimTypes.System)?.Value),
                 Or);
        }

        [HttpGet]
        public async Task<List<ComparisonChartSingleSourceReasonDto>> GetComparisonChartSingleSourceReasons()
        {
            var result = await new ComparisonChartLogic().GetComparisonChartSingleSourceReasons();
            return result;
        }

        [HttpGet("{comparisonId}/{approveStageMainId}")]
        public async Task<ComparisonChartGettingDatas> GetComparisonChartDatas(int comparisonId, int approveStageMainId)
        {
            var result = await new ComparisonChartLogic().GetComparisonChartGettingDatasAsync(comparisonId, approveStageMainId, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, false);
            return result;
        }

        [HttpGet("{comparisonId}/{tabIndex}")]
        public async Task<ComparisonChartGettingDatas> GetChartDatasForTabIndex(int comparisonId, byte tabIndex)
        {
            var result = await new ComparisonChartLogic().GetComparisonChartDatasByTabIndex(comparisonId, tabIndex, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }

        [HttpGet("{entityId}/{isComparison}")]
        public async Task<List<AttachmentModel>> GetBidAttachments(int entityId, bool isComparison)
        {
            var result = await new ComparisonChartLogic().GetBidAttachments(entityId, isComparison);
            return result;
        }

        [HttpGet("{comparisonId}")]
        public async Task<ApiResult> Resend(int comparisonId)
        {
            var result = await new ComparisonChartLogic().Resend(comparisonId, _env.WebRootPath);
            return result;
        }

        [HttpGet("{comparisonId}")]
        public async Task<ExportModel> ExportComparisonChart(int comparisonId)
        {
            var result = await new ComparisonChartLogic()
                .ExportComparisonChart(
                comparisonId, User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                _env.WebRootPath,
                User.FindFirst(ClaimTypes.System)?.Value
                );
            return result;
        }

        //POST Methods

        //Chart Operations Start
        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> SaveMessage(ChartMessageDto message)
        {
            return await new ComparisonChartLogic().SaveMessage(message, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, _env.WebRootPath);
        }

        [HttpPost]
        public async Task ChatSeened(IntegerSingleId comparisonChartId)
        {
            await new ComparisonChartLogic().ChatSeened(comparisonChartId.Id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<UnReadedChartAndWaitingForApprovalCountModel> GetWaitingForApprovalCountAndUnreadedMessages()
        {
            return await new ComparisonChartLogic().GetWaitingForApprovalCountAndUnreadedMessages(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst(ClaimTypes.System)?.Value);
        }

        [HttpGet("{comparisonChartId}")]
        public async Task<List<ChartGetMessagesDto>> GetChartMessages(int comparisonChartId)
        {
            return await new ComparisonChartLogic().GetChartMessages(comparisonChartId, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet("{comparisonChartId}")]
        public async Task<List<KeyValueTextBoxingDto>> GetComparisonChartUserList(int comparisonChartId)
        {
            return await new ComparisonChartLogic().GetComparisonChartUserList(comparisonChartId);
        }

        //Chart Operations End

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> SendToApprove(ComparisonChartCreateModelDto comparisonChart)
        {
            var result = await new ComparisonChartLogic().SaveAndSendToApprove(
                comparisonChart,
                ModelState,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                _env.WebRootPath
                );
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> Revise(IntegerSingleId comparisonChartId)
        {
            var result = await new ComparisonChartLogic().Revise(comparisonChartId.Id);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> SaveApprove(ApproveDataWithComment approveDataModel)
        {
            var result = await new ComparisonChartLogic().SaveApproveDatas(
                approveDataModel,
                ModelState,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                _env.WebRootPath,
                User.FindFirst(ClaimTypes.System)?.Value
                );
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> Reject(ComparisonChartRejectHoldModel rejectModel)
        {
            var result = await new ComparisonChartLogic().Reject(rejectModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, _env.WebRootPath);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> Hold(ComparisonChartRejectHoldModel holdModel)
        {
            var result = await new ComparisonChartLogic().Hold(holdModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, _env.WebRootPath);
            return result;
        }

        [HttpPost]
        public async Task<ApiResult> RestoreToDraft(IntegerSingleId comparisonId)
        => await new ComparisonChartLogic().RestoreToDraft(comparisonId);

        [HttpPost]
        public async Task<ApiResult> RealiseToSyteline(IntegerSingleId comparisonChartId)
       => await new ComparisonChartLogic().RealiseToSyteline(comparisonChartId.Id, User.FindFirst(ClaimTypes.System)?.Value, _env.WebRootPath);
    }
}
