using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.Dtos.Report;
using SolaBid.Business.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ReportController : ControllerBase
    {
        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<List<NameValueModel>> SingleSourceReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = await new ReportLogic().GetSingleSourceReport
                (dateFrom,
                 dateTo,
                 User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                 User.FindFirst(ClaimTypes.System)?.Value);
            return result;
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<List<NameValueModel>> BIDsStatusReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = await new ReportLogic().GetBIDsStatusReport
                (dateFrom,
                 dateTo,
                 User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                 User.FindFirst(ClaimTypes.System)?.Value);
            return result;
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public List<NameCountStatusModel> AverageProceedDurationReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = new ReportLogic().GetAverageProceedDurationReportReport
                    (dateFrom,
                     dateTo,
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public List<NameCountStatusModel> ComparisonDeadlineReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = new ReportLogic().GetComparisonDeadlineReportReport
                    (dateFrom,
                     dateTo,
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<List<NameDiscountTotal>> DiscountReportReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = await new ReportLogic().GetDiscountReport
                    (dateFrom,
                     dateTo,
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                     User.FindFirst(ClaimTypes.System)?.Value);
            return result;
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<List<CostSavedReportModel>> CostReport(DateTime dateFrom, DateTime dateTo)
        {
            var result = await new ReportLogic().GetCostReport
                    (dateFrom,
                     dateTo,
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                     User.FindFirst(ClaimTypes.System)?.Value);
            return result.OrderByDescending(m=>m.TotalBidAmount).ToList();
        }
    }
}
