using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ConnectableEntityExtensions;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Domain.Models.Entities;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using static SolaBid.Business.Logics.CommonLogic.GetData;
using static SolaBid.Business.Logics.ComparisonChartLogic;
using BIDComparisonGetDto = SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos.BIDComparisonGetDto;
using SolaBid.Domain.Models;

namespace SolaBid.Business.Logics
{
    public class ComparisonChartLogic
    {

        public async Task<ApiResult> RestoreToDraft(IntegerSingleId comparisonId)
        {


            var apiResult = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {

                var modBidComparison = await context.BIDComparisons
                    .Include(m => m.ComparisonChart)
                    .Where(m => m.Id == comparisonId.Id)
                    .FirstOrDefaultAsync();
                if (modBidComparison.ComparisonChart is null)
                {
                    apiResult.OperationIsSuccess = true;
                    return apiResult;
                }
                var modBidChart = await context.ComparisonCharts.FindAsync(modBidComparison.ComparisonChart.Id);
                var modChartBaseInfos = await context.ComparisonChartApprovalBaseInfos.Where(m => m.ComparisonChartId == modBidChart.Id).ToListAsync();
                var modBidReferances = await context.BIDReferances.Where(m => m.BIDComparisonId == modBidComparison.Id).ToListAsync();
                var modChartSingleSources = await context.RELComparisonChartSingleSources.Where(m => m.ComparisonChartId == modBidChart.Id).ToListAsync();
                context.ComparisonChartApprovalBaseInfos.RemoveRange(modChartBaseInfos);
                context.RELComparisonChartSingleSources.RemoveRange(modChartSingleSources);
                context.ComparisonCharts.Remove(modBidChart);
                foreach (var bRef in modBidReferances)
                {
                    bRef.StatusId = (byte)Statuses.Draft;
                    bRef.ApproveStatusId = (byte)ApproveStatuses.NotApproved;
                    bRef.WonStatusId = (byte)WonStatuses.InProgress;
                    context.Update(bRef);
                }
                await context.SaveChangesAsync();
            }
            apiResult.OperationIsSuccess = true;
            return apiResult;
        }

        public async Task<List<ORDetailDto>> OrDetail(string siteDatabase, string Or)
        {
            var result = await Task.Run<List<ORDetailDto>>(() =>
           {
               var res = FromQuery($"EXEC dbo.APT_OR_Details '{Or}','{siteDatabase}'", false).ConvertToClassListModel<ORDetailDto>();
               return res;
           });
            return result;
        }


        //public void aa()
        //{
        //    var result = new List<ComparisonChartMainListData>();

        //    var acceptedComparisonResultList = new List<BIDComparison>();
        //    var site = await new SiteLogic().GetSite(siteId.ToString());
        //    using (var context = TransactionConfig.AppDbContext)
        //    {
        //        var currentUser = await context.Users.FindAsync(userId);
        //        var userGroupBuyers = await GetUserGroupBuyersByUserId(userId, siteDatabase);
        //        var userGroupSiteWarehouses = await GetUserGroupSiteWarehouses(userId, siteId);
        //        Stopwatch stopwatchWOQuery = Stopwatch.StartNew();

        //        var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
        //                .Include(m => m.BIDRequest)
        //                .Where(m => userGroupBuyers.Contains(m.ComparisonChartPrepared))
        //                .OrderByDescending(m => m.ComparisonChart.EditedDate)
        //                .ToListAsync();
        //        stopwatchWOQuery.Stop();
        //        var resWOQ = stopwatchWOQuery.Elapsed;
        //        foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
        //        {
        //            var _comparisonReferance = await context.BIDReferances
        //                .Include(m => m.ApproveStatus)
        //                .Include(m => m.Status)
        //                .Where(m => m.BIDComparisonId == bidComparison.Id).FirstOrDefaultAsync();

        //            if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && m.SiteId == siteId))
        //            {
        //                acceptedComparisonResultList.Add(bidComparison);
        //            }
        //            if (currentUser.BuyerUserName == bidComparison.ComparisonChartPrepared && _comparisonReferance.StatusId == (byte)Statuses.Open || _comparisonReferance.StatusId == (byte)Statuses.OnHold)
        //            {
        //                var chartMainData = new ComparisonChartMainListData
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Site = site.SiteName,
        //                    ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
        //                    Buyer = bidComparison.ComparisonChartPrepared,
        //                    ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd/MM/yyyy"),
        //                    ComparisonNumber = bidComparison.ComparisonNumber,
        //                    Destination = _comparisonReferance.Destination,
        //                    PoNumber = String.Join(",", context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id && m.PONumber != null).Select(m => m.PONumber)),
        //                    Status = _comparisonReferance.Status.StatusName,
        //                    RequestDate = _comparisonReferance.RequestDate.ToString("dd/MM/yyyy"),
        //                    Requester = _comparisonReferance.Requester,
        //                    RequestNo = bidComparison.BIDRequest.RequestNumber,
        //                    RevisionNumber = bidComparison.ReviseNumber.ToString(),
        //                    SingleSource = context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id).Count() > 1 ? false : true,
        //                    ComparisonId = bidComparison.Id
        //                };
        //                chartMainData.ComparsionStatus = 5;
        //                result.Add(chartMainData);
        //            }
        //            _comparisonReferance = null;
        //        }

        //        foreach (var resultComparison in acceptedComparisonResultList)
        //        {
        //            var _comparisonReferance = await context.BIDReferances.Where(m => m.BIDComparisonId == resultComparison.Id)
        //                .Include(m => m.ApproveStatus)
        //                .Include(m => m.Status)
        //                .FirstOrDefaultAsync();
        //            var _comparisonAllReferances = await context.BIDReferances
        //                .Include(m => m.Vendor)
        //                .Where(m => m.BIDComparisonId == resultComparison.Id).ToListAsync();

        //            var _comparisonChart = await context.ComparisonCharts
        //                .Include(m => m.SingleSourceReasons).ThenInclude(m => m.ComparisonChartSingleSourceReason)
        //                .Where(m => m.BIDComparisonId == resultComparison.Id)
        //                .FirstOrDefaultAsync();
        //            var chartMainData = new ComparisonChartMainListData
        //            {
        //                Id = Guid.NewGuid(),
        //                Site = site.SiteName,
        //                ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
        //                Buyer = resultComparison.ComparisonChartPrepared,
        //                ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd/MM/yyyy"),
        //                ComparisonNumber = resultComparison.ComparisonNumber,
        //                Destination = _comparisonReferance.Destination + " / " + _comparisonReferance.OR,
        //                PoNumber = String.Join(",", _comparisonAllReferances.Select(m => m.PONumber).ToList()),
        //                Status = _comparisonReferance.Status.StatusName,
        //                RequestDate = _comparisonReferance.RequestDate.ToString("dd/MM/yyyy"),
        //                Requester = _comparisonReferance.Requester,
        //                RequestNo = resultComparison.BIDRequest.RequestNumber,
        //                RevisionNumber = resultComparison.ReviseNumber.ToString(),
        //                SingleSource = _comparisonAllReferances.Count() > 1 ? false : true,
        //                ComparisonId = resultComparison.Id
        //            };
        //            if (_comparisonChart == null)
        //            {
        //                chartMainData.ApprovalStatus = "Not Approved";

        //                if (_comparisonReferance.UserId == userId)
        //                {
        //                    chartMainData.ComparsionStatus = 4;
        //                    chartMainData.ComparisonCreateDate = resultComparison.CreateDate;
        //                }
        //            }
        //            else
        //            {
        //                chartMainData.ComparisonChartId = _comparisonChart.Id;
        //                var appCount = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id).Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Count();
        //                var appStageDetails = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                                .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).ToListAsync();
        //                var appStageDateData = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                                    .SelectMany(m => m.ComparisonChartApprovalBaseInfos).ToListAsync();
        //                for (int i = 1; i <= appCount; i++)
        //                {
        //                    var Approval = appStageDetails.Where(m => m.Sequence == i).Select(m => m.ApproveStageDetailName).First();

        //                    var ApprovalDate = appStageDateData?.Where(m => m.ApproveStageDetail.Sequence == i).FirstOrDefault()
        //                                     == null ? " " :
        //                                     appStageDateData.Where(m => m.ApproveStageDetail.Sequence == i).Select(m => m.ApproveDate).First().ToString("dd/MM/yyyy");
        //                    switch (i)
        //                    {
        //                        case 1:
        //                            {
        //                                chartMainData.FirstApproval = Approval;
        //                                chartMainData.FirstApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 2:
        //                            {
        //                                chartMainData.SecondApproval = Approval;
        //                                chartMainData.SecondApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 3:
        //                            {
        //                                chartMainData.ThirdApproval = Approval;
        //                                chartMainData.ThirdApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 4:
        //                            {
        //                                chartMainData.FourthApproval = Approval;
        //                                chartMainData.FourthApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 5:
        //                            {
        //                                chartMainData.FifthApproval = Approval;
        //                                chartMainData.FifthApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 6:
        //                            {
        //                                chartMainData.SixApproval = Approval;
        //                                chartMainData.SixApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 7:
        //                            {
        //                                chartMainData.SevenApproval = Approval;
        //                                chartMainData.SevenApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 8:
        //                            {
        //                                chartMainData.EightApproval = Approval;
        //                                chartMainData.EightApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 9:
        //                            {
        //                                chartMainData.NineApproval = Approval;
        //                                chartMainData.NineApprovalDate = ApprovalDate;
        //                                break;
        //                            }

        //                        case 10:
        //                            {
        //                                chartMainData.TenApproval = Approval;
        //                                chartMainData.TenApprovalDate = ApprovalDate;
        //                                break;
        //                            }
        //                    }
        //                }
        //                if (_comparisonChart.SingleSourceReasons.Any())
        //                {
        //                    chartMainData.SingleSourceReason = string.Join<string>(",", _comparisonChart.SingleSourceReasons.Select(m => m.ComparisonChartSingleSourceReason.SingleSourceReasonName));
        //                }
        //                if (!_comparisonChart.IsRealisedToSyteLine)
        //                {
        //                    var copyDataForRealised = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                    copyDataForRealised.ComparsionStatus = 6;
        //                    var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                    chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                }
        //                if (_comparisonChart.ApproveStatusId == (byte)ApproveStatuses.Approved)
        //                {
        //                    chartMainData.ComparsionStatus = 2;
        //                    var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                    chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                    continue;
        //                }
        //                if (_comparisonReferance.ApproveStatusId == (byte)ApproveStatuses.Rejected)
        //                {
        //                    chartMainData.ComparsionStatus = 3;
        //                    var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                    copyDataInfo.ComparsionStatus = 2;
        //                    var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                    chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                    continue;
        //                }
        //                if (_comparisonReferance.StatusId == (byte)Statuses.OnHold)
        //                {
        //                    chartMainData.ComparsionStatus = 5;
        //                    chartMainData.CommentProcurement = _comparisonChart.ComProcurementSpecialist;
        //                    var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                    chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                    var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                    copyDataInfo.ComparsionStatus = 2;
        //                    continue;
        //                }


        //                var maxBidReferanceValue = _comparisonAllReferances.Max(m => m.TotalAmount);

        //                var currentApprovals = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                                .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Where(m => m.Sequence == _comparisonChart.Stage);

        //                var selectedGroups = currentApprovals.SelectMany(m => m.ApproveRoleApproveStageDetails)
        //                    .Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
        //                    .Select(m => m.ApproveRole)
        //                    .SelectMany(m => m.GroupApproveRoles)
        //                    .Select(m => m.AppRoleId).Distinct();
        //                var approvedUserIsExist = (await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync()).Any(m => m == userId);

        //                if (approvedUserIsExist)
        //                {
        //                    chartMainData.ComparsionStatus = 1;
        //                    var minVedor = resultComparison.BIDReferances.Where(m => m.USDTotal == resultComparison.BIDReferances.Min(mv => mv.USDTotal)).First();
        //                    chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                }
        //            }

        //            if (_comparisonChart != null)
        //            {

        //                var copyData = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                copyData.ComparsionStatus = 2;

        //                chartMainData = copyData = null;
        //                _comparisonReferance = null;
        //            }
        //        }

        //    }
        //    return result;
        //}


        public async Task<List<ComparisonChartMainListData>> WaitingForApprovals(string userId)
        {
            return FromQuery($"exec [dbo].[SP_BIDComparisonMainWFA] '{userId}'", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> All()
        {
            return FromQuery("exec [dbo].[SP_BIDComparisonMainAll]", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> NotRealised()
        {
            return FromQuery("exec [dbo].[SP_BIDComparisonMainNotRealised]", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> Drafts()
        {
            return FromQuery("exec [dbo].[SP_BIDComparisonMainDraft]", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> Held()
        {
            return FromQuery("exec [dbo].[SP_BIDComparisonMainHeld]", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> Rejected()
        {
            return FromQuery("exec [dbo].[SP_BIDComparisonMainRejected]", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<List<ComparisonChartMainListData>> MyCharts(string userId)
        {
            return FromQuery($"exec [dbo].[SP_BIDComparisonMainMyCharts] '{userId}'", false).ConvertToClassListModel<ComparisonChartMainListData>();
        }

        public async Task<ComparisonChartTabDatas2> GetList(string userId, string siteDatabase, int siteId)
        {
            try
            {
                var result = new ComparisonChartTabDatas2();
                var acceptedComparisonResultList = new List<BIDComparison>();
                var site = await new SiteLogic().GetSite(siteId.ToString());
                using (var context = TransactionConfig.AppDbContext)
                {
                    var currentUser = await context.Users.FindAsync(userId);
                    var userGroupBuyers = await GetUserGroupBuyersByUserId(userId, siteDatabase);
                    var userGroupSiteWarehouses = await GetUserGroupSiteWarehouses(userId, siteId);
                    Stopwatch stopwatchWOQuery = Stopwatch.StartNew();

                    var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
                            .Include(m => m.BIDRequest)
                            .Where(m => userGroupBuyers.Contains(m.ComparisonChartPrepared))
                            .OrderByDescending(m => m.ComparisonChart.EditedDate)
                            .ToListAsync();
                    stopwatchWOQuery.Stop();
                    var resWOQ = stopwatchWOQuery.Elapsed;
                    foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
                    {
                        var _comparisonReferance = await context.BIDReferances
                            .Include(m => m.ApproveStatus)
                            .Include(m => m.Status)
                            .Where(m => m.BIDComparisonId == bidComparison.Id).FirstOrDefaultAsync();

                        if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && m.SiteId == siteId))
                        {
                            acceptedComparisonResultList.Add(bidComparison);
                        }
                        if (currentUser.BuyerUserName == bidComparison.ComparisonChartPrepared && _comparisonReferance.StatusId == (byte)Statuses.Open || _comparisonReferance.StatusId == (byte)Statuses.OnHold)
                        {
                            var chartMainData = new ComparisonChartMainListData2
                            {
                                Id = Guid.NewGuid(),
                                Site = site.SiteName,
                                ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
                                Buyer = bidComparison.ComparisonChartPrepared,
                                ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString(),
                                ComparisonNumber = bidComparison.ComparisonNumber,
                                Destination = _comparisonReferance.Destination,
                                PoNumber = String.Join(",", context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id && m.PONumber != null).Select(m => m.PONumber)),
                                Status = _comparisonReferance.Status.StatusName,
                                RequestDate = _comparisonReferance.RequestDate.ToString(),
                                Requester = _comparisonReferance.Requester,
                                RequestNo = bidComparison.BIDRequest.RequestNumber,
                                RevisionNumber = bidComparison.ReviseNumber,
                                SingleSource = context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id).Count() > 1 ? false : true,
                                ComparisonId = bidComparison.Id
                            };
                            chartMainData.ComparsionStatus = 5;
                            result.MyCharts.Add(chartMainData);
                        }
                        _comparisonReferance = null;
                    }

                    foreach (var resultComparison in acceptedComparisonResultList)
                    {
                        if (resultComparison.Id == 1303)
                        {

                        }
                        var _comparisonReferance = await context.BIDReferances.Where(m => m.BIDComparisonId == resultComparison.Id)
                            .Include(m => m.ApproveStatus)
                            .Include(m => m.Status)
                            .FirstOrDefaultAsync();
                        var _comparisonAllReferances = await context.BIDReferances
                            .Include(m => m.Vendor)
                            .Where(m => m.BIDComparisonId == resultComparison.Id).ToListAsync();

                        var _comparisonChart = await context.ComparisonCharts
                            .Include(m => m.SingleSourceReasons).ThenInclude(m => m.ComparisonChartSingleSourceReason)
                            .Where(m => m.BIDComparisonId == resultComparison.Id)
                            .FirstOrDefaultAsync();
                        var chartMainData = new ComparisonChartMainListData2
                        {
                            Id = Guid.NewGuid(),
                            Site = site.SiteName,
                            ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
                            Buyer = resultComparison.ComparisonChartPrepared,
                            ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString(),
                            ComparisonNumber = resultComparison.ComparisonNumber,
                            Destination = _comparisonReferance.Destination + " / " + _comparisonReferance.OR,
                            PoNumber = String.Join(",", _comparisonAllReferances.Select(m => m.PONumber).ToList()),
                            Status = _comparisonReferance.Status.StatusName,
                            RequestDate = _comparisonReferance.RequestDate.ToString(),
                            Requester = _comparisonReferance.Requester,
                            RequestNo = resultComparison.BIDRequest.RequestNumber,
                            RevisionNumber = resultComparison.ReviseNumber,
                            SingleSource = _comparisonAllReferances.Count() > 1 ? false : true,
                            ComparisonId = resultComparison.Id
                        };
                        if (_comparisonChart == null)
                        {
                            chartMainData.ApprovalStatus = "Not Approved";

                            if (_comparisonReferance.UserId == userId)
                            {
                                chartMainData.ComparsionStatus = 4;
                                chartMainData.ComparisonCreateDate = resultComparison.CreateDate.ToString();
                                result.Drafts.Add(chartMainData);
                            }
                        }
                        else
                        {
                            chartMainData.ComparisonChartId = _comparisonChart.Id;
                            var appCount = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id).Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Count();
                            var appStageDetails = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
                                            .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).ToListAsync();
                            var appStageDateData = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
                                                .SelectMany(m => m.ComparisonChartApprovalBaseInfos).ToListAsync();
                            for (int i = 1; i <= appCount; i++)
                            {
                                var Approval = appStageDetails.Where(m => m.Sequence == i).Select(m => m.ApproveStageDetailName).First();

                                var ApprovalDate = appStageDateData?.Where(m => m.ApproveStageDetail.Sequence == i).FirstOrDefault()
                                                 == null ? " " :
                                                 appStageDateData.Where(m => m.ApproveStageDetail.Sequence == i).Select(m => m.ApproveDate).First().ToString("dd/MM/yyyy");
                                switch (i)
                                {
                                    case 1:
                                        {
                                            chartMainData.FirstApproval = Approval;
                                            chartMainData.FirstApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 2:
                                        {
                                            chartMainData.SecondApproval = Approval;
                                            chartMainData.SecondApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 3:
                                        {
                                            chartMainData.ThirdApproval = Approval;
                                            chartMainData.ThirdApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 4:
                                        {
                                            chartMainData.FourthApproval = Approval;
                                            chartMainData.FourthApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 5:
                                        {
                                            chartMainData.FifthApproval = Approval;
                                            chartMainData.FifthApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 6:
                                        {
                                            chartMainData.SixApproval = Approval;
                                            chartMainData.SixApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 7:
                                        {
                                            chartMainData.SevenApproval = Approval;
                                            chartMainData.SevenApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 8:
                                        {
                                            chartMainData.EightApproval = Approval;
                                            chartMainData.EightApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 9:
                                        {
                                            chartMainData.NineApproval = Approval;
                                            chartMainData.NineApprovalDate = ApprovalDate;
                                            break;
                                        }

                                    case 10:
                                        {
                                            chartMainData.TenApproval = Approval;
                                            chartMainData.TenApprovalDate = ApprovalDate;
                                            break;
                                        }
                                }
                            }
                            if (_comparisonChart.SingleSourceReasons.Any())
                            {
                                chartMainData.SingleSourceReason = string.Join<string>(",", _comparisonChart.SingleSourceReasons.Select(m => m.ComparisonChartSingleSourceReason.SingleSourceReasonName));
                            }
                            if (!_comparisonChart.IsRealisedToSyteLine)
                            {
                                var copyDataForRealised = TransactionConfig.Mapper.Map<ComparisonChartMainListData2>(chartMainData);
                                copyDataForRealised.ComparsionStatus = 6;
                                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
                                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
                                result.NotRealised.Add(copyDataForRealised);
                            }
                            if (_comparisonChart.ApproveStatusId == (byte)ApproveStatuses.Approved)
                            {
                                chartMainData.ComparsionStatus = 2;
                                result.AllComparisons.Add(chartMainData);
                                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
                                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
                                continue;
                            }
                            if (_comparisonReferance.ApproveStatusId == (byte)ApproveStatuses.Rejected)
                            {
                                chartMainData.ComparsionStatus = 3;
                                result.Rejected.Add(chartMainData);
                                var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData2>(chartMainData);
                                copyDataInfo.ComparsionStatus = 2;
                                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
                                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
                                result.AllComparisons.Add(copyDataInfo);
                                continue;
                            }
                            if (_comparisonReferance.StatusId == (byte)Statuses.OnHold)
                            {
                                chartMainData.ComparsionStatus = 5;
                                chartMainData.CommentProcurement = _comparisonChart.ComProcurementSpecialist;
                                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
                                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
                                result.Holded.Add(chartMainData);
                                var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData2>(chartMainData);
                                copyDataInfo.ComparsionStatus = 2;
                                result.AllComparisons.Add(copyDataInfo);
                                continue;
                            }


                            var maxBidReferanceValue = _comparisonAllReferances.Max(m => m.TotalAmount);

                            var currentApprovals = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
                                            .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Where(m => m.Sequence == _comparisonChart.Stage);

                            var selectedGroups = currentApprovals.SelectMany(m => m.ApproveRoleApproveStageDetails)
                                .Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                                .Select(m => m.ApproveRole)
                                .SelectMany(m => m.GroupApproveRoles)
                                .Select(m => m.AppRoleId).Distinct();
                            var approvedUserIsExist = (await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync()).Any(m => m == userId);

                            if (approvedUserIsExist)
                            {
                                chartMainData.ComparsionStatus = 1;
                                var minVedor = resultComparison.BIDReferances.Where(m => m.USDTotal == resultComparison.BIDReferances.Min(mv => mv.USDTotal)).First();
                                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
                                result.WaitingForApproval.Add(chartMainData);
                            }
                        }

                        if (_comparisonChart != null)
                        {
                            var copyData = chartMainData.DeepCopy();
                            copyData.ComparsionStatus = 2;
                            result.AllComparisons.Add(copyData);

                            chartMainData = copyData = null;
                            _comparisonReferance = null;
                        }
                    }

                }
                result.Drafts = result.Drafts.OrderByDescending(m => m.ComparisonCreateDate).ToList();
                return result;
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        #region list


        //var result = new ComparisonChartTabDatas();
        //var acceptedComparisonResultList = new List<BIDComparison>();
        //var site = await new SiteLogic().GetSite(siteId.ToString());
        //using (var context = TransactionConfig.AppDbContext)
        //{
        //    var currentUser = await context.Users.FindAsync(userId);
        //    var userGroupBuyers = await GetUserGroupBuyersByUserId(userId, siteDatabase);
        //    var userGroupSiteWarehouses = await GetUserGroupSiteWarehouses(userId, siteId);
        //    Stopwatch stopwatchWOQuery = Stopwatch.StartNew();

        //    var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
        //            .Include(m => m.BIDRequest)
        //            .Where(m => userGroupBuyers.Contains(m.ComparisonChartPrepared))
        //            .OrderByDescending(m => m.ComparisonChart.EditedDate)
        //            .ToListAsync();
        //    stopwatchWOQuery.Stop();
        //    var resWOQ = stopwatchWOQuery.Elapsed;
        //    foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
        //    {
        //        var _comparisonReferance = await context.BIDReferances
        //            .Include(m => m.ApproveStatus)
        //            .Include(m => m.Status)
        //            .Where(m => m.BIDComparisonId == bidComparison.Id).FirstOrDefaultAsync();

        //        if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && m.SiteId == siteId))
        //        {
        //            acceptedComparisonResultList.Add(bidComparison);
        //        }
        //        if (currentUser.BuyerUserName == bidComparison.ComparisonChartPrepared && _comparisonReferance.StatusId == (byte)Statuses.Open || _comparisonReferance.StatusId == (byte)Statuses.OnHold)
        //        {
        //            var chartMainData = new ComparisonChartMainListData
        //            {
        //                Id = Guid.NewGuid(),
        //                Site = site.SiteName,
        //                ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
        //                Buyer = bidComparison.ComparisonChartPrepared,
        //                ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd/MM/yyyy"),
        //                ComparisonNumber = bidComparison.ComparisonNumber,
        //                Destination = _comparisonReferance.Destination,
        //                PoNumber = String.Join(",", context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id && m.PONumber != null).Select(m => m.PONumber)),
        //                Status = _comparisonReferance.Status.StatusName,
        //                RequestDate = _comparisonReferance.RequestDate.ToString("dd/MM/yyyy"),
        //                Requester = _comparisonReferance.Requester,
        //                RequestNo = bidComparison.BIDRequest.RequestNumber,
        //                RevisionNumber = bidComparison.ReviseNumber.ToString(),
        //                SingleSource = context.BIDReferances.Where(m => m.BIDComparisonId == bidComparison.Id).Count() > 1 ? false : true,
        //                ComparisonId = bidComparison.Id
        //            };
        //            chartMainData.ComparsionStatus = 5;
        //            result.MyCharts.Add(chartMainData);
        //        }
        //        _comparisonReferance = null;
        //    }

        //    foreach (var resultComparison in acceptedComparisonResultList)
        //    {
        //        var _comparisonReferance = await context.BIDReferances.Where(m => m.BIDComparisonId == resultComparison.Id)
        //            .Include(m => m.ApproveStatus)
        //            .Include(m => m.Status)
        //            .FirstOrDefaultAsync();
        //        var _comparisonAllReferances = await context.BIDReferances
        //            .Include(m => m.Vendor)
        //            .Where(m => m.BIDComparisonId == resultComparison.Id).ToListAsync();

        //        var _comparisonChart = await context.ComparisonCharts
        //            .Include(m => m.SingleSourceReasons).ThenInclude(m => m.ComparisonChartSingleSourceReason)
        //            .Where(m => m.BIDComparisonId == resultComparison.Id)
        //            .FirstOrDefaultAsync();
        //        var chartMainData = new ComparisonChartMainListData
        //        {
        //            Id = Guid.NewGuid(),
        //            Site = site.SiteName,
        //            ApprovalStatus = _comparisonReferance.ApproveStatus.StatusName,
        //            Buyer = resultComparison.ComparisonChartPrepared,
        //            ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd/MM/yyyy"),
        //            ComparisonNumber = resultComparison.ComparisonNumber,
        //            Destination = _comparisonReferance.Destination + " / " + _comparisonReferance.OR,
        //            PoNumber = String.Join(",", _comparisonAllReferances.Select(m => m.PONumber).ToList()),
        //            Status = _comparisonReferance.Status.StatusName,
        //            RequestDate = _comparisonReferance.RequestDate.ToString("dd/MM/yyyy"),
        //            Requester = _comparisonReferance.Requester,
        //            RequestNo = resultComparison.BIDRequest.RequestNumber,
        //            RevisionNumber = resultComparison.ReviseNumber.ToString(),
        //            SingleSource = _comparisonAllReferances.Count() > 1 ? false : true,
        //            ComparisonId = resultComparison.Id
        //        };
        //        if (_comparisonChart == null)
        //        {
        //            chartMainData.ApprovalStatus = "Not Approved";

        //            if (_comparisonReferance.UserId == userId)
        //            {
        //                chartMainData.ComparsionStatus = 4;
        //                chartMainData.ComparisonCreateDate = resultComparison.CreateDate;
        //                result.Drafts.Add(chartMainData);
        //            }
        //        }
        //        else
        //        {
        //            chartMainData.ComparisonChartId = _comparisonChart.Id;
        //            var appCount = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id).Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Count();
        //            var appStageDetails = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                            .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).ToListAsync();
        //            var appStageDateData = await context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                                .SelectMany(m => m.ComparisonChartApprovalBaseInfos).ToListAsync();
        //            for (int i = 1; i <= appCount; i++)
        //            {
        //                var Approval = appStageDetails.Where(m => m.Sequence == i).Select(m => m.ApproveStageDetailName).First();

        //                var ApprovalDate = appStageDateData?.Where(m => m.ApproveStageDetail.Sequence == i).FirstOrDefault()
        //                                 == null ? " " :
        //                                 appStageDateData.Where(m => m.ApproveStageDetail.Sequence == i).Select(m => m.ApproveDate).First().ToString("dd/MM/yyyy");
        //                switch (i)
        //                {
        //                    case 1:
        //                        {
        //                            chartMainData.FirstApproval = Approval;
        //                            chartMainData.FirstApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 2:
        //                        {
        //                            chartMainData.SecondApproval = Approval;
        //                            chartMainData.SecondApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 3:
        //                        {
        //                            chartMainData.ThirdApproval = Approval;
        //                            chartMainData.ThirdApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 4:
        //                        {
        //                            chartMainData.FourthApproval = Approval;
        //                            chartMainData.FourthApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 5:
        //                        {
        //                            chartMainData.FifthApproval = Approval;
        //                            chartMainData.FifthApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 6:
        //                        {
        //                            chartMainData.SixApproval = Approval;
        //                            chartMainData.SixApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 7:
        //                        {
        //                            chartMainData.SevenApproval = Approval;
        //                            chartMainData.SevenApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 8:
        //                        {
        //                            chartMainData.EightApproval = Approval;
        //                            chartMainData.EightApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 9:
        //                        {
        //                            chartMainData.NineApproval = Approval;
        //                            chartMainData.NineApprovalDate = ApprovalDate;
        //                            break;
        //                        }

        //                    case 10:
        //                        {
        //                            chartMainData.TenApproval = Approval;
        //                            chartMainData.TenApprovalDate = ApprovalDate;
        //                            break;
        //                        }
        //                }
        //            }
        //            if (_comparisonChart.SingleSourceReasons.Any())
        //            {
        //                chartMainData.SingleSourceReason = string.Join<string>(",", _comparisonChart.SingleSourceReasons.Select(m => m.ComparisonChartSingleSourceReason.SingleSourceReasonName));
        //            }
        //            if (!_comparisonChart.IsRealisedToSyteLine)
        //            {
        //                var copyDataForRealised = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                copyDataForRealised.ComparsionStatus = 6;
        //                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                result.NotRealised.Add(copyDataForRealised);
        //            }
        //            if (_comparisonChart.ApproveStatusId == (byte)ApproveStatuses.Approved)
        //            {
        //                chartMainData.ComparsionStatus = 2;
        //                result.AllComparisons.Add(chartMainData);
        //                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                continue;
        //            }
        //            if (_comparisonReferance.ApproveStatusId == (byte)ApproveStatuses.Rejected)
        //            {
        //                chartMainData.ComparsionStatus = 3;
        //                result.Rejected.Add(chartMainData);
        //                var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                copyDataInfo.ComparsionStatus = 2;
        //                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                result.AllComparisons.Add(copyDataInfo);
        //                continue;
        //            }
        //            if (_comparisonReferance.StatusId == (byte)Statuses.OnHold)
        //            {
        //                chartMainData.ComparsionStatus = 5;
        //                chartMainData.CommentProcurement = _comparisonChart.ComProcurementSpecialist;
        //                var minVedor = _comparisonAllReferances.Where(m => m.USDTotal == _comparisonAllReferances.Min(mv => mv.USDTotal)).First();
        //                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                result.Holded.Add(chartMainData);
        //                var copyDataInfo = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //                copyDataInfo.ComparsionStatus = 2;
        //                result.AllComparisons.Add(copyDataInfo);
        //                continue;
        //            }


        //            var maxBidReferanceValue = _comparisonAllReferances.Max(m => m.TotalAmount);

        //            var currentApprovals = context.ComparisonCharts.Where(m => m.Id == _comparisonChart.Id)
        //                            .Select(m => m.ApproveStage).SelectMany(m => m.ApproveStageDetails).Where(m => m.Sequence == _comparisonChart.Stage);

        //            var selectedGroups = currentApprovals.SelectMany(m => m.ApproveRoleApproveStageDetails)
        //                .Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
        //                .Select(m => m.ApproveRole)
        //                .SelectMany(m => m.GroupApproveRoles)
        //                .Select(m => m.AppRoleId).Distinct();
        //            var approvedUserIsExist = (await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync()).Any(m => m == userId);

        //            if (approvedUserIsExist)
        //            {

        //                chartMainData.ComparsionStatus = 1;
        //                var minVedor = resultComparison.BIDReferances.Where(m => m.USDTotal == resultComparison.BIDReferances.Min(mv => mv.USDTotal)).First();
        //                chartMainData.CheapestPrice = minVedor.USDTotal.ToString() + " " + _comparisonReferance.Currency + " - " + minVedor.Vendor.VendorName;
        //                result.WaitingForApproval.Add(chartMainData);
        //            }
        //        }

        //        if (_comparisonChart != null)
        //        {

        //            var copyData = TransactionConfig.Mapper.Map<ComparisonChartMainListData>(chartMainData);
        //            copyData.ComparsionStatus = 2;
        //            result.AllComparisons.Add(copyData);

        //            chartMainData = copyData = null;
        //            _comparisonReferance = null;
        //        }
        //    }

        //}
        //int resulc = result.WaitingForApproval.Count;
        //result.Drafts = result.Drafts.OrderByDescending(m => m.ComparisonCreateDate).ToList();
        //return result;
        #endregion
        public async Task<ApiResult> RealiseToSyteline(int comparisonChartId, string siteId, string root)
        {
            var res = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedChart = await context.ComparisonCharts
                    .Where(m => m.Id == comparisonChartId)
                    .FirstOrDefaultAsync();
                if (approvedChart != null)
                {
                    var wonnedVendorIds = await context.BIDReferances
                          .Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId && m.WonStatusId == (byte)WonStatuses.Won)
                          .Select(m => m.VendorId)
                          .ToListAsync();
                    await new VendorLogic().VendorSendSiteLineAsync(siteId, wonnedVendorIds);
                    var wonnedBidIds = await context.BIDReferances
                        .Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId && m.WonStatusId == (byte)WonStatuses.Won)
                        .Select(m => m.Id)
                        .ToListAsync();
                    var PONum = await WonnedBidsSendSiteLineAsync(siteId, wonnedBidIds);
                    _ = SendRealiseToSyteLineComplatedEmail(approvedChart.BIDComparisonId, root, PONum);
                    approvedChart.IsRealisedToSyteLine = true;
                    _ = context.SaveChangesAsync();
                    res.OperationIsSuccess = true;
                    res.Data = PONum;
                }
                else
                {
                    res.ErrorList.Add("Bid is not valid");
                }
            }

            return res;
        }

        public async Task<List<KeyValueTextBoxingDto>> GetComparisonChartUserList(int comparisonChartId)
        {
            var result = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {

                var comparisonChart = await context.ComparisonCharts
                    .Include(i => i.BIDComparison).ThenInclude(m => m.BIDReferances)
                    .Include(i => i.CreatedUser)
                    .Where(m => m.Id == comparisonChartId).FirstAsync();

                var _comparisonReferance = comparisonChart.BIDComparison.BIDReferances.FirstOrDefault();
                var createdUser = comparisonChart.CreatedUser;
                var chartWarehouse = _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim();
                var warehouseAcceptedGroups = await context.GroupSiteWarehouses
                    .Where(m => m.WarehouseCode == chartWarehouse)
                    .Select(m => m.AppRoleId).ToListAsync();
                var buyerAcceptedGroups = await context.GroupBuyers
                    .Where(m => m.BuyerId == int.Parse(createdUser.BuyerId))
                    .Select(m => m.AppRoleId).ToListAsync();
                var acceptedGroups = warehouseAcceptedGroups.Where(m => buyerAcceptedGroups.Contains(m)).ToList();
                var acceptedUserIds = await context.UserRoles.Where(m => acceptedGroups.Contains(m.RoleId)).Select(m => m.UserId).Distinct().ToListAsync();
                var acceptedUserEntities = await context.Users.Where(m => acceptedUserIds.Contains(m.Id)).ToListAsync();
                foreach (var user in acceptedUserEntities.Select((user, index) => new { index, user }))
                {
                    result.Add(new KeyValueTextBoxingDto
                    {
                        Key = user.index.ToString(),
                        Text = user.user.FirstName + " " + user.user.LastName,
                        Value = user.user.Email
                    });
                }
            }
            return result;
        }

        public async Task<List<ChartGetMessagesDto>> GetChartMessages(int comparisonChartId, string userId)
        {
            var result = new List<ChartGetMessagesDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var chartMessages = await context.ComparisonChartChats
                    .Include(m => m.User)
                    .Where(m => m.ComparisonChartId == comparisonChartId)
                    .OrderBy(m => m.OrderNumber).ToListAsync();
                foreach (var message in chartMessages)
                {
                    result.Add(new ChartGetMessagesDto
                    {
                        FullName = message.User.FirstName + " " + message.User.LastName,
                        IsRight = message.UserId == userId,
                        Message = message.Message,
                        SendedDate = message.SendedDate.ToString("dd/MM/yyyy HH:mm")
                    });
                }
            }
            return result;
        }

        public async Task<UnReadedChartAndWaitingForApprovalCountModel> GetWaitingForApprovalCountAndUnreadedMessages(string userId, string siteId)
        {
            var site = await new SiteLogic().GetSite(siteId);
            var userComparisonList = await GetList(userId, site.SiteDatabase, site.Id);

            var result = new UnReadedChartAndWaitingForApprovalCountModel();
            result.WaitingForApprovalCount = userComparisonList.WaitingForApproval.Count;

            List<int> userWaitingChartIds = userComparisonList.WaitingForApproval.Select(m => m.ComparisonChartId).ToList();
            List<int> userRejectedChartIds = userComparisonList.Rejected.Select(m => m.ComparisonChartId).ToList();
            List<int> userHoldedChartIds = userComparisonList.Holded.Select(m => m.ComparisonChartId).ToList();

            var userChartIds = new List<int>(userWaitingChartIds.Count + userRejectedChartIds.Count + userHoldedChartIds.Count);
            userChartIds.AddRange(userWaitingChartIds);
            userChartIds.AddRange(userRejectedChartIds);
            userChartIds.AddRange(userHoldedChartIds);

            using (var context = TransactionConfig.AppDbContext)
            {
                var lastMessagesList = new List<ComparisonChartChat>();
                var lastViewedCharts = await context.ComparisonChartChatUserLastViews.Where(m => m.UserId == userId).ToListAsync();
                foreach (var userChartId in userChartIds)
                {
                    var lastMessage = await context.ComparisonChartChats
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.BIDComparison)
                        .Where(m => m.ComparisonChartId == userChartId)
                        .OrderByDescending(m => m.Id)
                        .Take(1)
                        .FirstOrDefaultAsync();
                    if (lastMessage is not null)
                    {
                        lastMessagesList.Add(lastMessage);
                    }
                }
                foreach (var lastMessageData in lastMessagesList)
                {
                    var detectChartIsReaded = lastViewedCharts.Where(m => m.ComparisonChartId == lastMessageData.ComparisonChartId).FirstOrDefault();
                    if (detectChartIsReaded == null || detectChartIsReaded?.ViewDate < lastMessageData.SendedDate)
                    {
                        string parametr = userWaitingChartIds.Any(m => m == lastMessageData.ComparisonChartId) | userHoldedChartIds.Any(m => m == lastMessageData.ComparisonChartId)
                            ? "1"
                            : userRejectedChartIds.Any(m => m == lastMessageData.ComparisonChartId)
                            ? "3"
                            : "2";
                        result.UnreadedCharts.Add(new UnreadedChatModel
                        {
                            BidComparisonNumber = lastMessageData.ComparisonChart.BIDComparison.ComparisonNumber,
                            UnReadedChartParametr = lastMessageData.ComparisonChart.BIDComparisonId + "/" + parametr
                        });
                    }
                }
            }
            return result;
        }

        public async Task ChatSeened(int comparisonChartId, string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {

                ComparisonChartChatUserLastView existSeenedData = await context.ComparisonChartChatUserLastViews
                    .Where(m => m.ComparisonChartId == comparisonChartId && m.UserId == userId)
                    .FirstOrDefaultAsync();

                if (existSeenedData is null)
                {
                    string ChartCompariosonNumber = await context.ComparisonCharts
                  .Where(m => m.Id == comparisonChartId)
                  .Select(m => m.BIDComparison.ComparisonNumber)
                  .FirstOrDefaultAsync();
                    await context.ComparisonChartChatUserLastViews.AddAsync(new ComparisonChartChatUserLastView
                    {
                        ComparisonChartId = comparisonChartId,
                        UserId = userId,
                        ComparisonNumber = ChartCompariosonNumber,
                        ViewDate = DateTime.Now
                    });
                }
                else
                {
                    existSeenedData.ViewDate = DateTime.Now;
                    context.ComparisonChartChatUserLastViews.Update(existSeenedData);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<ApiResult> SaveMessage(ChartMessageDto message, string userId, string root)
        {
            var result = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {

                int orderNum = await context.ComparisonChartChats.AnyAsync(m => m.ComparisonChartId == message.ComparisonChartId)
                    ?
                    (await context.ComparisonChartChats
                    .Where(m => m.ComparisonChartId == message.ComparisonChartId)
                    .MaxAsync(m => m.OrderNumber)) + 1
                    :
                    1;

                await context.ComparisonChartChats.AddAsync(new ComparisonChartChat
                {
                    ComparisonChartId = message.ComparisonChartId,
                    UserId = userId,
                    Message = message.Message,
                    SendedDate = DateTime.Now,
                    OrderNumber = orderNum
                });
                var isExistChartReaded = await context.ComparisonChartChatUserLastViews
                    .Where(m => m.ComparisonChartId == message.ComparisonChartId && m.UserId == userId)
                    .FirstOrDefaultAsync();
                if (isExistChartReaded is not null)
                {
                    isExistChartReaded.ViewDate = DateTime.Now.AddSeconds(3);
                    context.ComparisonChartChatUserLastViews.Update(isExistChartReaded);
                }


                var modifiedChart = await context.ComparisonCharts
                      .Include(m => m.CreatedUser)
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDReferances)
                      .FirstOrDefaultAsync(m => m.Id == message.ComparisonChartId);
                var _modifiedChartReferance = modifiedChart.BIDComparison.BIDReferances.First();
                var senderBuyer = await context.Users.FindAsync(userId);

                var urlParametr = _modifiedChartReferance.ApproveStatusId == (byte)ApproveStatuses.Rejected
                    ? "3"
                    : _modifiedChartReferance.StatusId == (byte)Statuses.OnHold
                    ? "1"
                    : "1";

                if (string.IsNullOrEmpty(message.AttachedBuyerEmail))
                {
                    modifiedChart.ResponsiblePersonId = modifiedChart.CreatedUserId;
                    modifiedChart.ResponsibilityDate = DateTime.Now;
                    modifiedChart.ResponsiblePerson = modifiedChart.CreatedUser.FirstName + " " + modifiedChart.CreatedUser.LastName;
                    if (senderBuyer.Id != modifiedChart.CreatedUserId)
                    {
                        new Thread(() =>
                        {
                            SendNewMessageInfo(
                       modifiedChart.CreatedUser.Email,
                       modifiedChart.BIDComparison.ComparisonNumber,
                       modifiedChart.BIDComparisonId + "/" + urlParametr,
                       modifiedChart.CreatedUser.FirstName + " " + modifiedChart.CreatedUser.LastName,
                       message.Message,
                       senderBuyer.FirstName + " " + senderBuyer.LastName,
                       DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                            );
                        }).Start();

                    }
                }
                else
                {
                    var attachedBuyer = await context.Users.FirstOrDefaultAsync(m => m.Email == message.AttachedBuyerEmail);

                    modifiedChart.ResponsiblePersonId = attachedBuyer.Id;
                    modifiedChart.ResponsibilityDate = DateTime.Now;
                    modifiedChart.ResponsiblePerson = attachedBuyer.FirstName + " " + attachedBuyer.LastName;
                    new Thread(() =>
                    {
                        SendNewMessageInfo(
                        message.AttachedBuyerEmail,
                        modifiedChart.BIDComparison.ComparisonNumber,
                        modifiedChart.BIDComparisonId + "/" + urlParametr,
                        attachedBuyer.FirstName + " " + attachedBuyer.LastName,
                        message.Message,
                        senderBuyer.FirstName + " " + senderBuyer.LastName,
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        );
                    }).Start();

                }

                context.ComparisonCharts.Update(modifiedChart);
                await context.SaveChangesAsync();

                void SendNewMessageInfo(string to, string comparisonNumber, string urlParametr, string reciverfullname, string message, string senderfullname, string sendDate)
                {
                    var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                    MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart New Message - Comparison № {comparisonNumber}");
                    mail.To.Add(to);
                    mail.Subject = $"BID Comparison Chart New Message - Comparison № {comparisonNumber}";
                    mail.IsBodyHtml = true;
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartNewMessage.html")))
                    { body = reader.ReadToEnd(); }
                    body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{urlParametr}");
                    body = body.Replace("{comparisonnumber}", comparisonNumber);
                    body = body.Replace("{message}", message);
                    body = body.Replace("{reciverfullname}", reciverfullname);
                    body = body.Replace("{senderfullname}", senderfullname);
                    body = body.Replace("{senddate}", sendDate);
                    mail.Body = body;

                    using (var sc = new SmtpClient())
                    {
                        sc.Port = 587;
                        sc.Host = "mail.apertech.net";
                        sc.EnableSsl = true;
                        sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                        sc.Send(mail);
                    }
                }
            }
            result.OperationIsSuccess = true;
            return result;
        }

        public async Task<List<GroupSiteWarehouse>> GetUserGroupSiteWarehouses(string userId, int siteId)
        {
            var warehouseAllList = new List<GroupSiteWarehouse>();
            var userGroups = await new GroupLogic().GetUserGroupIdsByUserId(userId);
            foreach (var userGroupId in userGroups)
            {
                warehouseAllList.AddRange(await new SiteLogic().GetGroupSiteWarehouses(userGroupId, siteId));
            }
            return warehouseAllList.CustomDistinctBy(m => m.WarehouseCode).ToList();
        }
        public async Task<List<string>> GetUserGroupBuyersByUserId(string userId, string siteDatabase)
        {
            var siteLineBuyers = new SiteLineDbLogic(siteDatabase).GetBuyers();

            var userBuyers = new List<string>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userRolesEntity = await context.UserRoles.Where(m => m.UserId == userId).ToListAsync();
                var groupEntities = await context.Roles
                        .Include(m => m.GroupBuyers)
                        .ToListAsync();

                foreach (var role in userRolesEntity)
                {
                    var groupEntity = groupEntities
                        .Where(m => m.Id == role.RoleId)
                        .SelectMany(m => m.GroupBuyers)
                        .Select(m => m.BuyerId);

                    userBuyers.AddRange(siteLineBuyers.Where(m => groupEntity.Contains((int)m.Id)).Select(m => m.Username).ToList());
                }
            }
            return userBuyers.Distinct().ToList();
        }
        public async Task<ApiResult> SaveAndSendToApprove(ComparisonChartCreateModelDto _comparisonChart, ModelStateDictionary ModelState, string userId, string root)
        {
            var apiResult = new ApiResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var newComparisonChart = new ComparisonChart
                {
                    ApproveStageId = _comparisonChart.ApproveStageId,
                    BIDComparisonId = _comparisonChart.BidComparisonId,
                    ComProcurementSpecialist = _comparisonChart.Comment,
                    CreatedDate = DateTime.Now,
                    EditedDate = DateTime.Now,
                    CreatedUserId = userId,
                    EditedUserId = userId,
                    SingleSource = _comparisonChart.SingleSource,
                    StatusId = (byte)Statuses.Open,
                    ApproveStatusId = (byte)ApproveStatuses.Stage1,
                    Stage = 1,
                    WonnedLineTotalUSD = "0",
                    WonnedLineTotalAZN = "0",
                    Annex = _comparisonChart.Annex,
                };

                try
                {
                    await context.ComparisonCharts.AddAsync(newComparisonChart);
                    var comparisonBidReferances = await context.BIDReferances.Where(m => m.BIDComparisonId == _comparisonChart.BidComparisonId).ToListAsync();
                    foreach (var bidRef in comparisonBidReferances)
                    {
                        bidRef.StatusId = (byte)Statuses.Open;
                        bidRef.ApproveStatusId = (byte)ApproveStatuses.Stage1;
                        context.BIDReferances.Update(bidRef);
                    }
                    await context.SaveChangesAsync();
                    //Added Single Source Reason
                    if (_comparisonChart.SingleSourceDatas.Count > 0)
                    {
                        var singelSourceResons = new List<RELComparisonChartSingleSource>();
                        _comparisonChart.SingleSourceDatas.ForEach(reasonId =>
                        {
                            singelSourceResons.Add(new RELComparisonChartSingleSource
                            {
                                ComparisonChartId = newComparisonChart.Id,
                                ComparisonChartSingleSourceReasonId = reasonId
                            });
                        });
                        await context.RELComparisonChartSingleSources.AddRangeAsync(singelSourceResons);
                        await context.SaveChangesAsync();
                    }

                    _ = SendApproveEmailRequest(newComparisonChart.BIDComparisonId, root);


                    apiResult.OperationIsSuccess = true;
                    return apiResult;
                }
                catch (Exception ex)
                {
                    context.ComparisonCharts.Remove(newComparisonChart);
                    await context.SaveChangesAsync();
                    apiResult.ErrorList.Add(ex.Message);
                    return apiResult;
                }

            }
        }
        public async Task<ComparisonChartGettingDatas> GetComparisonChartGettingDatasAsync(int comparionId, int approveStageMainId, string userId, bool isAllComparison)
        {
            ComparisonChartGettingDatas result = null;
            using (var context = TransactionConfig.AppDbContext)
            {
                var userCanSeeAmounts = await new AdditionalPrivilegeLogic().UserCanSeeAmount(userId);

                var _comparison = await context.BIDComparisons
                    .Include(m => m.BIDReferances)
                    .Include(m => m.BIDReferances).ThenInclude(m => m.Site)
                    .Include(m => m.BIDReferances).ThenInclude(m => m.Atachments)
                    .Include(m => m.BIDReferances).ThenInclude(m => m.Vendor)
                    .Include(m => m.BIDReferances).ThenInclude(m => m.RequestItems)
                    .Include(m => m.BIDRequest)
                    .Where(m => m.Id == comparionId)
                    .FirstOrDefaultAsync();

                if (_comparison == null)
                    return null;
                var _comparisonReferance = _comparison.BIDReferances.FirstOrDefault();
                var approveStageDetails = TransactionConfig.Mapper.Map<List<ApproveStageDetailDto>>(await context.ApproveStageDetails
                    .Where(m => m.ApproveStageMainId == approveStageMainId)
                    .OrderBy(m => m.Sequence)
                    .ToListAsync());

                result = new ComparisonChartGettingDatas
                {
                    Buyer = _comparison.ComparisonChartPrepared,
                    ComparisonDate = _comparisonReferance.ComparisonDate.ToString("dd.MM.yyyy"),
                    ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd.MM.yyyy"),
                    ComparisonNumber = _comparison.ComparisonNumber,
                    Destination = _comparisonReferance.Destination,
                    Entity = _comparisonReferance.Site.SiteName,
                    Requester = _comparisonReferance.Requester,
                    RequestNo = _comparison.BIDRequest.RequestNumber,
                    OneTimePo = true,
                    Annex = false,
                    OR = _comparisonReferance.OR,
                    WarehouseName = _comparisonReferance.ProjectWarehouse,
                    WonLastPurchasedPriceUSD = "N/A",
                    SingleSourceReason = "",
                    WonnerVendorAndLines = "",
                    WonTotalAZN = "",
                    WonTotalUSD = "",
                    Statuses = await GetStatusesKV(),
                    ApprovalStatuses = await GetApprovalStatusKV(),
                    ApprovalStages = await GetApprovalStagesKV(),
                    StatusId = (byte)Statuses.Draft,
                    ApproveStatusId = (byte)ApproveStatuses.NotApproved,
                    ApprovalStageId = "0",
                    CanRevise = false,
                    ApprovalStageDetails = approveStageDetails
                };
                //var userBuer = await context.Users.FindAsync(userId);
                //result.isChartPrepared = _comparison.ComparisonChartPrepared == userBuer.BuyerUserName;
                var comparisonItemsBySiteLine = new SiteLineDbLogic(_comparisonReferance.Site.SiteDatabase).GetRequestLines(_comparisonReferance.Site.SiteName, _comparison.BIDRequest.RequestNumber);
                foreach (var reqItem in _comparisonReferance.RequestItems)
                {
                    var reqItemBySiteLine = comparisonItemsBySiteLine.Where(m => m.RowPointer == reqItem.RowPointer).FirstOrDefault();
                    if (reqItemBySiteLine != null)
                    {
                        result.RequestInformations.Add(new RequestInformationDto
                        {
                            Budget = reqItemBySiteLine.Budget == 0 ? "0" : reqItemBySiteLine.Budget.ToString("#.##"),
                            RowPointer = reqItemBySiteLine.RowPointer,
                            DescriptionOfRequiredPurchase = reqItemBySiteLine.ItemName,
                            LastPurchasedDate = "N/A",
                            LastPurchasedPrice = "N/A",
                            PRItemNo = reqItemBySiteLine.RequestLine,
                            RequestQuantity = reqItemBySiteLine.Quantity.ToString("#.##"),
                            UOM = reqItemBySiteLine.UOM
                        });
                    }
                }
                result.RequestInformations = result.RequestInformations.OrderBy(m => m.PRItemNo).ToList();
                for (int i = 0; i < result.RequestInformations.Count; i++)
                {
                    result.RequestInformations[i].OrderNumber = i;
                }
                var minUSDBestPrice = _comparison.BIDReferances.Min(m => m.USDTotal);
                foreach (var comparisonBidReferance in _comparison.BIDReferances)
                {
                    var copyAppStageDetails = TransactionConfig.Mapper.Map<List<ApproveStageDetailDto>>(approveStageDetails);

                    copyAppStageDetails.ForEach(m => m.BidReferanceId = comparisonBidReferance.Id);
                    var addedInfo = new BIDReferanceInformation()
                    {
                        BIDReferanceId = comparisonBidReferance.Id,
                        BudgetBalance = comparisonBidReferance.BudgetBalance.ToString("#.##"),
                        ExpectedDelivery = comparisonBidReferance.ExpectedDelivery.ToString("#.##"),
                        EntryDateFormatted = comparisonBidReferance.EntryDate.ToString("dd.MM.yyyy"),
                        BIDReferanceNumber = comparisonBidReferance.BIDNumber,
                        Currency = comparisonBidReferance.Currency,
                        DeliveryTerms = comparisonBidReferance.DeliveryTerm + "-" + comparisonBidReferance.DeliveryDescription,
                        DeliveryTime = comparisonBidReferance.DeliveryTime,
                        PaymentTerms = comparisonBidReferance.PayementTerm + "-" + comparisonBidReferance.PaymentDescription,
                        TotalAmount = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.TotalAmount.ToString("#.##") : "0"
                                                      : comparisonBidReferance.TotalAmount.ToString("#.##"),
                        TotalAZN = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.AZNTotal == 0 ? "0" : comparisonBidReferance.AZNTotal.ToString("#.##") : "0"
                                                    : comparisonBidReferance.AZNTotal == 0 ? "0" : comparisonBidReferance.AZNTotal.ToString("#.##"),
                        TotalUSD = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.USDTotal == 0 ? "0" : comparisonBidReferance.USDTotal.ToString("#.##") : "0"
                                                    : comparisonBidReferance.USDTotal == 0 ? "0" : comparisonBidReferance.USDTotal.ToString("#.##"),
                        DiscountValue = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.DiscountValue == 0 ? "0" : comparisonBidReferance.DiscountValue.ToString("#.##") : "0"
                                                    : comparisonBidReferance.DiscountValue == 0 ? "0" : comparisonBidReferance.DiscountValue.ToString("#.##"),
                        DiscountPrice = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.DiscountPrice == 0 ? "0" : comparisonBidReferance.DiscountPrice.ToString("#.##") : "0"
                                                    : comparisonBidReferance.DiscountPrice == 0 ? "0" : comparisonBidReferance.DiscountPrice.ToString("#.##"),
                        DiscountedTotalPrice = isAllComparison ? userCanSeeAmounts ? comparisonBidReferance.DiscountedTotalPrice == 0 ? "0" : comparisonBidReferance.DiscountedTotalPrice.ToString("#.##") : "0"
                                                    : comparisonBidReferance.DiscountedTotalPrice == 0 ? "0" : comparisonBidReferance.DiscountedTotalPrice.ToString("#.##"),
                        VendorName = comparisonBidReferance.Vendor.VendorName,
                        HasAttachment = comparisonBidReferance.Atachments.Count > 0 ? true : false,
                        IsWon = comparisonBidReferance.WonStatusId == (byte)WonStatuses.Won ? true : false,
                        ApproveStageDetails = copyAppStageDetails,
                        IsBestPrice = minUSDBestPrice == 0 ? false
                            :
                            comparisonBidReferance.USDTotal == minUSDBestPrice
                            ?
                            true : false,
                    };
                    var sorterRequestItems = TransactionConfig.Mapper.Map<List<RELComparisonRequestItemDto>>(comparisonBidReferance.RequestItems);
                    sorterRequestItems.ForEach(m => m.OrderNum = result.RequestInformations.Where(r => r.RowPointer == m.RowPointer).Select(s => s.OrderNumber).First());
                    sorterRequestItems = sorterRequestItems.OrderBy(m => m.OrderNum).ToList();
                    foreach (var reqItemByCompBidRef in sorterRequestItems)
                    {
                        var minUnitPriceDatas = _comparison.BIDReferances
                            .SelectMany(m => m.RequestItems)
                            .Where(m => m.RowPointer == reqItemByCompBidRef.RowPointer && m.UniqueCurrencyUnitPrice > 0)
                            .ToList();


                        addedInfo.BIDInformationItems.Add(new BIDInformationItems
                        {
                            EntryDateFormatted = comparisonBidReferance.EntryDate.ToString("dd.MM.yyyy"),
                            BidComment = reqItemByCompBidRef.LineDescription,
                            BidQuantity = reqItemByCompBidRef.Quantity,
                            RowPointer = reqItemByCompBidRef.RowPointer,
                            TotalPrice = isAllComparison ? userCanSeeAmounts ? reqItemByCompBidRef.TotalPrice == 0 ? "0" : reqItemByCompBidRef.TotalPrice.ToString("#.##") : "0"
                                                                             : reqItemByCompBidRef.TotalPrice == 0 ? "0" : reqItemByCompBidRef.TotalPrice.ToString("#.##"),
                            UnitPrice = isAllComparison ? userCanSeeAmounts ? reqItemByCompBidRef.UnitPrice == 0 ? "0" : reqItemByCompBidRef.UnitPrice.ToString("#.##") : "0"
                                                                             : reqItemByCompBidRef.UnitPrice == 0 ? "0" : reqItemByCompBidRef.UnitPrice.ToString("#.##"),
                            Discount = isAllComparison ? userCanSeeAmounts ? reqItemByCompBidRef.Discount == 0 ? "0" : reqItemByCompBidRef.Discount.ToString("#.##") : "0"
                                                                             : reqItemByCompBidRef.Discount == 0 ? "0" : reqItemByCompBidRef.Discount.ToString("#.##"),
                            LineTotalDiscount = isAllComparison ? userCanSeeAmounts ? reqItemByCompBidRef.LineTotalDiscount == 0 ? "0" : reqItemByCompBidRef.LineTotalDiscount.ToString("#.##") : "0"
                                                                             : reqItemByCompBidRef.LineTotalDiscount == 0 ? "0" : reqItemByCompBidRef.LineTotalDiscount.ToString("#.##"),
                            IsBestUnitPrice = minUnitPriceDatas.Count == 0 ? false : reqItemByCompBidRef.UniqueCurrencyUnitPrice == minUnitPriceDatas.Min(m => m.UniqueCurrencyUnitPrice),
                            Conv = reqItemByCompBidRef.Conv,
                            ConvQuantity = reqItemByCompBidRef.ConvQuantity,
                            ConvUnitPrice = userCanSeeAmounts ? reqItemByCompBidRef.ConvUnitPrice : 0,
                            PUOMValue = reqItemByCompBidRef.PUOMValue
                        });
                    }
                    result.BidReferanceInformations.Add(addedInfo);
                }
            }
            return result;
        }
        public async Task<ComparisonChartGettingDatas> GetComparisonChartApproveDatasAsync(int comparionId, string userId)
        {

            ComparisonChartGettingDatas result = null;
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var userCanSeeAmounts = await new AdditionalPrivilegeLogic().UserCanSeeAmount(userId);
                    var _comparison = await context.BIDComparisons.Where(m => m.Id == comparionId).FirstOrDefaultAsync();
                    _comparison.BIDReferances = context.BIDReferances.Where(x => x.BIDComparisonId == _comparison.Id)
                        .Include(x => x.Vendor)
                        .Include(x => x.ApproveStatus)
                        .Include(x => x.Site)
                        .Include(x => x.Atachments)
                        .Include(x => x.RequestItems)
                        .ToList();
                    _comparison.BIDRequest = await context.BIDRequests.Where(m => m.Id == _comparison.BIDRequestId).FirstOrDefaultAsync();
                    _comparison.ComparisonChart = await context.ComparisonCharts.Where(x => x.BIDComparisonId == comparionId)
                        .Include(x => x.ComparisonChartApproveStages)
                        .Include(x => x.ComparisonChartApprovalBaseInfos)
                        .Include(x => x.SingleSourceReasons).ThenInclude(m => m.ComparisonChartSingleSourceReason)
                        .Include(x => x.ComparisonChartApprovalBaseInfos).ThenInclude(m => m.ApprovedUser)
                        .FirstOrDefaultAsync();
                    var _comparisonReferance = _comparison.BIDReferances.FirstOrDefault();
                    var userGroupsIds = await context.UserRoles.Where(m => m.UserId == userId).Select(m => m.RoleId).ToListAsync();
                    var CanRevise = (await context.GroupAdditionalPrivileges
                        .Where(m => userGroupsIds.Contains(m.AppRoleId) && m.AdditionalPrivilegeId == (byte)AdditionalPrivilegeses.CanRevise && _comparison.ComparisonChart.StatusId != (byte)Statuses.Closed && _comparison.ComparisonChart.StatusId != (byte)Statuses.Canceled)
                        .FirstOrDefaultAsync()) == null ? false : true;
                    if (_comparison == null)
                        return null;

                    var approveStageDetails = TransactionConfig.Mapper.Map<List<ApproveStageDetailDto>>(await context.ApproveStageDetails
                        .Where(m => m.ApproveStageMainId == _comparison.ComparisonChart.ApproveStageId)
                        .OrderBy(m => m.Sequence)
                        .ToListAsync());
                    var comparisonChartBaseInfos = _comparison.ComparisonChart.ComparisonChartApprovalBaseInfos;
                    foreach (var appStage in approveStageDetails)
                    {
                        var stageInfos = comparisonChartBaseInfos.Where(m => m.ApproveStageDetailId == appStage.Id).FirstOrDefault();
                        if (stageInfos != null)
                        {
                            appStage.NameSurname = stageInfos.ApprovedUser.FirstName + " " + stageInfos.ApprovedUser.LastName;
                            appStage.Comment = stageInfos.Comment;
                            appStage.ApproveDate = stageInfos.ApproveDate.ToString("dd.MM.yyyy");
                            appStage.TotalApprovedAmount = userCanSeeAmounts ? stageInfos.TotalApprovedAmount : "0";
                            appStage.ApprovedUserId = stageInfos.ApprovedUser.Id;
                            appStage.SignaturePath = stageInfos.ApprovedUser.UserSignature;
                            appStage.IsSelected = string.IsNullOrEmpty(appStage.ApproveDate) ? false : true;
                        }
                    }
                    //Wonner Informations Start
                    var singleSourceReasons = string.Join(',', _comparison.ComparisonChart?.SingleSourceReasons?.Select(m => m.ComparisonChartSingleSourceReason.SingleSourceReasonName).ToList());
                    var wonVendorNames = string.Empty;
                    if (_comparisonReferance.ApproveStatusId == (byte)ApproveStatuses.Approved)
                    {
                        wonVendorNames = string.Join(',', _comparison.BIDReferances.Where(m => m.WonStatusId == (byte)WonStatuses.Won).Select(m => m.Vendor.VendorName).ToList());
                    }

                    //Wonner Informations End
                    result = new ComparisonChartGettingDatas
                    {
                        Buyer = _comparison.ComparisonChartPrepared,
                        ComparisonDate = _comparisonReferance.ComparisonDate.ToString("dd.MM.yyyy"),
                        ComparisonDeadline = _comparisonReferance.ComparisonDeadline.ToString("dd.MM.yyyy"),
                        ComparisonNumber = _comparison.ComparisonNumber,
                        Destination = _comparisonReferance.Destination,
                        OR = _comparisonReferance.OR,
                        Entity = _comparisonReferance.Site.SiteName,
                        Requester = _comparisonReferance.Requester,
                        RequestNo = _comparison.BIDRequest.RequestNumber,
                        WarehouseName = _comparisonReferance.ProjectWarehouse,
                        WonLastPurchasedPriceUSD = userCanSeeAmounts ? "N/A" : "0",
                        ApproveStageName = approveStageDetails.Where(m => m.Sequence == _comparison.ComparisonChart.Stage).Select(m => m.ApproveStageDetailName).FirstOrDefault(),
                        ApproveStageDetailId = approveStageDetails.Where(m => m.Sequence == _comparison.ComparisonChart.Stage).Select(m => m.Id).FirstOrDefault(),
                        Stage = _comparison.ComparisonChart.Stage,
                        OneTimePo = !_comparison.ComparisonChart.Annex,
                        Annex = _comparison.ComparisonChart.Annex,
                        SingleSourceReason = singleSourceReasons,
                        WonnerVendorAndLines = wonVendorNames,
                        WonTotalAZN = userCanSeeAmounts ? _comparison.ComparisonChart.WonnedLineTotalAZN : "0",
                        WonTotalUSD = userCanSeeAmounts ? _comparison.ComparisonChart.WonnedLineTotalUSD : "0",
                        ComProcurementSpecialist = _comparison.ComparisonChart.ComProcurementSpecialist,
                        Statuses = await GetStatusesKV(),
                        ApprovalStatuses = await GetApprovalStatusKV(),
                        ApprovalStages = await GetApprovalStagesKV(),
                        ApprovalStageId = _comparison.ComparisonChart.ApproveStageId.ToString(),
                        ComparisonChartId = _comparison.ComparisonChart.Id.ToString(),
                        StatusId = _comparison.ComparisonChart.StatusId,
                        ApproveStatusId = _comparison.ComparisonChart.ApproveStatusId,
                        CanRevise = CanRevise,
                        ApprovalStageDetails = approveStageDetails,
                        LastSentMessageTo = _comparison.ComparisonChart.ResponsiblePerson,
                        LastUpdateMessageDate = _comparison.ComparisonChart.ResponsibilityDate == null ? null : _comparison.ComparisonChart.ResponsibilityDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                        ChatIsAviable = _comparison.ComparisonChart.ApproveStatusId != (byte)ApproveStatuses.Rejected && _comparison.ComparisonChart.ApproveStatusId != (byte)ApproveStatuses.Approved
                    };
                    //var userBuer = await context.Users.FindAsync(userId);
                    //result.isChartPrepared = _comparison.ComparisonChartPrepared == userBuer.BuyerUserName;
                    var comparisonItemsBySiteLine = new SiteLineDbLogic(_comparisonReferance.Site.SiteDatabase).GetRequestLines(_comparisonReferance.Site.SiteName, _comparison.BIDRequest.RequestNumber);
                    foreach (var reqItem in _comparisonReferance.RequestItems)
                    {
                        var reqItemBySiteLine = comparisonItemsBySiteLine.Where(m => m.RowPointer == reqItem.RowPointer).FirstOrDefault();
                        if (reqItemBySiteLine != null)
                        {
                            result.RequestInformations.Add(new RequestInformationDto
                            {
                                Budget = reqItemBySiteLine.Budget == 0 ? "0" : reqItemBySiteLine.Budget.ToString("#.##"),
                                RowPointer = reqItemBySiteLine.RowPointer,
                                DescriptionOfRequiredPurchase = reqItemBySiteLine.ItemName,
                                LastPurchasedDate = "N/A",
                                LastPurchasedPrice = "N/A",
                                PRItemNo = reqItemBySiteLine.BidLine,
                                RequestQuantity = reqItemBySiteLine.Quantity.ToString("#.##"),
                                UOM = reqItemBySiteLine.UOM
                            });
                        }
                    }
                    result.RequestInformations = result.RequestInformations.OrderBy(m => m.PRItemNo).ToList();
                    for (int i = 0; i < result.RequestInformations.Count; i++)
                    {
                        result.RequestInformations[i].OrderNumber = i;
                    }
                    var minUSDBestPrice = _comparison.BIDReferances.Min(m => m.USDTotal);
                    foreach (var comparisonBidReferance in _comparison.BIDReferances)
                    {
                        var copyAppStageDetails = TransactionConfig.Mapper.Map<List<ApproveStageDetailDto>>(approveStageDetails);
                        copyAppStageDetails.ForEach(m => m.BidReferanceId = comparisonBidReferance.Id);
                        var addedInfo = new BIDReferanceInformation()
                        {
                            BIDReferanceId = comparisonBidReferance.Id,
                            BIDReferanceNumber = comparisonBidReferance.BIDNumber,
                            BudgetBalance = comparisonBidReferance.BudgetBalance.ToString("#.##"),
                            ExpectedDelivery = comparisonBidReferance.ExpectedDelivery.ToString("#.##"),
                            Currency = comparisonBidReferance.Currency,
                            EntryDateFormatted = comparisonBidReferance.EntryDate.ToString("dd.MM.yyyy"),
                            DeliveryTerms = comparisonBidReferance.DeliveryTerm + "-" + comparisonBidReferance.DeliveryDescription,
                            DeliveryTime = comparisonBidReferance.DeliveryTime,
                            PaymentTerms = comparisonBidReferance.PayementTerm + "-" + comparisonBidReferance.PaymentDescription,
                            TotalAmount = userCanSeeAmounts ? comparisonBidReferance.TotalAmount.ToString("#.##") : "0",
                            TotalAZN = userCanSeeAmounts ? comparisonBidReferance.AZNTotal == 0 ? "0" : comparisonBidReferance.AZNTotal.ToString("#.##") : "0",
                            TotalUSD = userCanSeeAmounts ? comparisonBidReferance.USDTotal == 0 ? "0" : comparisonBidReferance.USDTotal.ToString("#.##") : "0",
                            DiscountedTotalPrice = userCanSeeAmounts ? comparisonBidReferance.DiscountedTotalPrice == 0 ? "0" : comparisonBidReferance.DiscountedTotalPrice.ToString("#.##") : "0",
                            DiscountPrice = userCanSeeAmounts ? comparisonBidReferance.DiscountPrice == 0 ? "0" : comparisonBidReferance.DiscountPrice.ToString("#.##") : "0",
                            DiscountValue = userCanSeeAmounts ? comparisonBidReferance.DiscountValue == 0 ? "0" : comparisonBidReferance.DiscountValue.ToString("#.##") : "0",
                            VendorName = comparisonBidReferance.Vendor.VendorName,
                            HasAttachment = comparisonBidReferance.Atachments.Count > 0 ? true : false,
                            ApproveStatusName = comparisonBidReferance.ApproveStatus.StatusName,
                            IsWon = comparisonBidReferance.WonStatusId == (byte)WonStatuses.Won ? true : false,
                            ApproveStageDetails = copyAppStageDetails,
                            IsBestPrice = minUSDBestPrice == 0 ? false
                            :
                            comparisonBidReferance.USDTotal == minUSDBestPrice
                            ?
                            true : false,
                        };

                        foreach (var appStage in approveStageDetails)
                        {
                            var appStageBidRefApprovedItemCount = _comparison.ComparisonChart.ComparisonChartApproveStages.Where(m => m.ApproveStageDetailId == appStage.Id && m.BidReferanceId == comparisonBidReferance.Id).Count();
                            var itemCount = _comparisonReferance.RequestItems.Count();
                            if (appStageBidRefApprovedItemCount >= 1)
                            {
                                var percent = Math.Round(Convert.ToDecimal((appStageBidRefApprovedItemCount * 100) / itemCount));
                                addedInfo.ApproveStageDetailsWithPercent.Add(new ApproveStageDetailPercentage
                                {
                                    Name = $"{appStage.ApproveStageDetailName} {itemCount}({appStageBidRefApprovedItemCount})",
                                    Percent = percent
                                });
                            }
                            else
                            {
                                addedInfo.ApproveStageDetailsWithPercent.Add(new ApproveStageDetailPercentage
                                {
                                    Name = $"{appStage.ApproveStageDetailName} {itemCount}({0})",
                                    Percent = 0
                                });
                            }

                        }
                        foreach (var item in comparisonBidReferance.RequestItems)
                        {
                            item.BIDReferance = null;
                        }

                        var sorterRequestItems = TransactionConfig.Mapper.Map<List<RELComparisonRequestItemDto>>(comparisonBidReferance.RequestItems);
                        sorterRequestItems.ForEach(m => m.OrderNum = result.RequestInformations.Where(r => r.RowPointer == m.RowPointer).Select(s => s.OrderNumber).First());
                        sorterRequestItems = sorterRequestItems.OrderBy(m => m.OrderNum).ToList();

                        foreach (var reqItemByCompBidRef in sorterRequestItems)
                        {
                            var reqItemBySiteLine = comparisonItemsBySiteLine.Where(m => m.RowPointer == reqItemByCompBidRef.RowPointer).FirstOrDefault();
                            var minUnitPriceDatas = _comparison.BIDReferances.SelectMany(m => m.RequestItems).Where(m => m.RowPointer == reqItemByCompBidRef.RowPointer && m.UniqueCurrencyUnitPrice > 0).ToList();



                            if (reqItemBySiteLine != null)
                            {
                                var bidInfoItem = new BIDInformationItems
                                {
                                    BidComment = reqItemByCompBidRef.LineDescription,
                                    BidQuantity = reqItemByCompBidRef.Quantity,
                                    RowPointer = reqItemByCompBidRef.RowPointer,
                                    EntryDateFormatted = comparisonBidReferance.EntryDate.ToString("dd.MM.yyyy"),
                                    TotalPrice = userCanSeeAmounts ? reqItemByCompBidRef.TotalPrice == 0 ? "0" : reqItemByCompBidRef.TotalPrice.ToString("#.##") : "0",
                                    UnitPrice = userCanSeeAmounts ? reqItemByCompBidRef.UnitPrice == 0 ? "0" : reqItemByCompBidRef.UnitPrice.ToString("#.##") : "0",
                                    Discount = userCanSeeAmounts ? reqItemByCompBidRef.Discount == 0 ? "0" : reqItemByCompBidRef.Discount.ToString("#.##") : "0",
                                    LineTotalDiscount = userCanSeeAmounts ? reqItemByCompBidRef.LineTotalDiscount == 0 ? "0" : reqItemByCompBidRef.LineTotalDiscount.ToString("#.##") : "0",
                                    DescriptionOfRequiredPurchase = reqItemBySiteLine.ItemName,
                                    BidReferanceId = comparisonBidReferance.Id,
                                    ApproveStageDetailId = result.ApproveStageDetailId,
                                    ComparisonChartId = _comparison.ComparisonChart.Id,
                                    Stage = result.Stage,
                                    IsBestUnitPrice = minUnitPriceDatas.Count == 0 ? false : reqItemByCompBidRef.UniqueCurrencyUnitPrice == minUnitPriceDatas.Min(m => m.UniqueCurrencyUnitPrice),
                                    Conv = reqItemByCompBidRef.Conv,
                                    ConvQuantity = reqItemByCompBidRef.ConvQuantity,
                                    ConvUnitPrice = userCanSeeAmounts ? reqItemByCompBidRef.ConvUnitPrice : 0,
                                    PUOMValue = reqItemByCompBidRef.PUOMValue
                                };
                                var currBidIsApproved = _comparison.ComparisonChart.ComparisonChartApproveStages.Where(m => m.BidReferanceId == comparisonBidReferance.Id).ToList();
                                foreach (var appStage in approveStageDetails)
                                {
                                    var a = appStage;
                                    var isApproved = false;
                                    if (currBidIsApproved != null &&
                                        currBidIsApproved.Count > 0 &&
                                        currBidIsApproved.Any(m => m.ApproveStageDetailId == a.Id && m.BidReferanceItemRowPointer == reqItemByCompBidRef.RowPointer)
                                        )
                                    {
                                        isApproved = true;
                                    }
                                    bidInfoItem.ApprovalStageDetails.Add(new ApproveDatasModel
                                    {
                                        ApproveStageDetailId = a.Id,
                                        BidReferanceId = comparisonBidReferance.Id,
                                        RowPointer = reqItemByCompBidRef.RowPointer,
                                        Stage = a.Sequence,
                                        ComparisonChartId = _comparison.ComparisonChart.Id,
                                        IsApproved = isApproved
                                    });
                                }
                                addedInfo.BIDInformationItems.Add(bidInfoItem);
                            }
                        }
                        result.IsDraftable = _comparisonReferance.StatusId == (byte)Statuses.OnHold;
                        result.BidReferanceInformations.Add(addedInfo);
                    }
                }

            }
            catch (Exception ex)
            {
                _ = ex.ErrorLog();
            }
            return result;
        }
        public async Task<ComparisonChartGettingDatas> GetComparisonChartAllComparisonDatas(int comparisonId, string userId)
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                var comparison = await context.BIDComparisons
                    .Include(m => m.ComparisonChart)
                    .Where(m => m.Id == comparisonId)
                    .FirstOrDefaultAsync();
                if (comparison.ComparisonChart == null)
                {
                    return await GetComparisonChartGettingDatasAsync(comparisonId, 0, userId, true);
                }
                else
                {
                    return await GetComparisonChartApproveDatasAsync(comparisonId, userId);
                }
            }
        }
        public async Task<List<KeyValueTextBoxingDto>> GetStatusesKV()
        {
            var result = new List<KeyValueTextBoxingDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var statusesEntity = await context.Statuses.ToListAsync();
                result.AddRange(statusesEntity.Select(item => new KeyValueTextBoxingDto
                {
                    Key = item.Id.ToString(),
                    Text = item.StatusName,
                    Value = item.Id.ToString()
                }));
            }
            return result;
        }
        public async Task<List<KeyValueTextBoxingDto>> GetApprovalStatusKV()
        {
            var result = new List<KeyValueTextBoxingDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var approvalStatusesEntity = await context.ApproveStatuses.ToListAsync();
                result.AddRange(approvalStatusesEntity.Select(item => new KeyValueTextBoxingDto
                {
                    Key = item.Id.ToString(),
                    Text = item.StatusName,
                    Value = item.Id.ToString()
                }));
            }
            return result;
        }
        public async Task<List<KeyValueTextBoxingDto>> GetApprovalStagesKV()
        {
            var result = new List<KeyValueTextBoxingDto>();

            using (var context = TransactionConfig.AppDbContext)
            {
                var approvalStageEntity = await context.ApproveStageMains.ToListAsync();
                result.AddRange(approvalStageEntity.Select(item => new KeyValueTextBoxingDto
                {
                    Key = item.Id.ToString(),
                    Text = item.ApproveStageName,
                    Value = item.Id.ToString()
                }));
            }
            return result;
        }
        public async Task<List<ComparisonChartSingleSourceReasonDto>> GetComparisonChartSingleSourceReasons()
        {
            var result = new List<ComparisonChartSingleSourceReasonDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var comparisonChartSingleSourceReasonEntity = await context.ComparisonChartSingleSourceReasons.ToListAsync();
                result = TransactionConfig.Mapper.Map<List<ComparisonChartSingleSourceReasonDto>>(comparisonChartSingleSourceReasonEntity);
            }
            return result;
        }
        public async Task<ApiResult> SaveApproveDatas(ApproveDataWithComment approveDataModel, ModelStateDictionary ModelState, string userId, string root, string siteId)
        {
            var apiResult = new ApiResult();
            var site = await new SiteLogic().GetSite(siteId);
            var hasAccessForApprove = await GetList(userId, site.SiteDatabase, site.Id);
            var userWaitingApproveDatas = hasAccessForApprove.WaitingForApproval;
            var userHoldedDatas = hasAccessForApprove.Holded;
            var firstApprovedRow = approveDataModel.ApproveDataModels.FirstOrDefault();
            if (firstApprovedRow == null)
            {
                apiResult.OperationIsSuccess = true;
                return apiResult;
            }
            using (var context = TransactionConfig.AppDbContext)
            {
                var approvedChart = await context.ComparisonCharts
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDReferances)
                    .Include(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                    .Where(m => m.Id == firstApprovedRow.ComparisonChartId)
                    .FirstOrDefaultAsync();
                var _firstBidRef = approvedChart.BIDComparison.BIDReferances.FirstOrDefault();
                if (!userWaitingApproveDatas.Any(m => m.ComparisonNumber == approvedChart.BIDComparison.ComparisonNumber) &&
                    !userHoldedDatas.Any(m => m.ComparisonNumber == approvedChart.BIDComparison.ComparisonNumber))
                {
                    apiResult.OperationIsSuccess = true;
                    return apiResult;
                }
                approvedChart.EditedDate = DateTime.Now;

                var approveStages = approvedChart.ApproveStage.ApproveStageDetails.Count();
                foreach (var apprDatas in approveDataModel.ApproveDataModels)
                {
                    await context.ComparisonChartApproveStages.AddAsync(new ComparisonChartApproveStage
                    {
                        ComparisonChartId = apprDatas.ComparisonChartId,
                        ApproveStageDetailId = apprDatas.ApproveStageDetailId,
                        BidReferanceId = apprDatas.BidReferanceId,
                        BidReferanceItemRowPointer = apprDatas.RowPointer,
                        Stage = apprDatas.Stage
                    });
                }
                await context.ComparisonChartApprovalBaseInfos.AddAsync(new ComparisonChartApprovalBaseInfos
                {
                    ApproveDate = DateTime.Now,
                    ApprovedUserId = userId,
                    ApproveStageDetailId = firstApprovedRow.ApproveStageDetailId,
                    Comment = approveDataModel.Comment,
                    ComparisonChartId = firstApprovedRow.ComparisonChartId,
                    TotalApprovedAmount = approveDataModel.ApprovalStageTotalPrice
                });

                if (approvedChart.Stage == approveStages) //Complated Stage Approve
                {
                    var convertedValute = new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN("USD", Convert.ToDecimal(approveDataModel.ApprovalStageTotalPrice), _firstBidRef.ComparisonDate.ToString("dd.MM.yyyy"), site.SiteName);
                    approvedChart.WonnedLineTotalAZN = convertedValute.AZN;
                    approvedChart.WonnedLineTotalUSD = convertedValute.USD;
                    approvedChart.Stage = 99;
                    approvedChart.ApproveStatusId = (byte)ApproveStatuses.Approved;
                    approvedChart.IsRealisedToSyteLine = DateTime.Now < _firstBidRef.ComparisonDeadline;

                    var comparisonBidReferances = await context.BIDReferances.Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId).ToListAsync();
                    foreach (var bidRef in comparisonBidReferances)
                    {
                        bidRef.ApproveStatusId = approvedChart.ApproveStatusId;
                        bidRef.StatusId = (byte)Statuses.Closed;
                        bidRef.WonStatusId = approveDataModel.ApproveDataModels.Any(m => m.BidReferanceId == bidRef.Id) ? (byte)WonStatuses.Won : (byte)WonStatuses.Loss;
                        bidRef.CEOApproveDateFormatted = bidRef.WonStatusId == (byte)WonStatuses.Won ? DateTime.Now.ToString("dd.MM.yyyy") : null;
                        context.BIDReferances.Update(bidRef);
                    }
                    try
                    {
                        await context.SaveChangesAsync();
                        apiResult.OperationIsSuccess = true;
                        if (DateTime.Now < _firstBidRef.ComparisonDeadline)
                        {
                            var wonnedVendorIds = await context.BIDReferances
                         .Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId && m.WonStatusId == (byte)WonStatuses.Won)
                         .Select(m => m.VendorId)
                         .ToListAsync();
                            await new VendorLogic().VendorSendSiteLineAsync(siteId, wonnedVendorIds);
                            var wonnedBidIds = await context.BIDReferances
                                .Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId && m.WonStatusId == (byte)WonStatuses.Won)
                                .Select(m => m.Id)
                                .ToListAsync();
                            var PONum = await WonnedBidsSendSiteLineAsync(siteId, wonnedBidIds);
                            _ = SendApproveComplatedEmail(approvedChart.BIDComparisonId, root, PONum);
                            apiResult.Data = "realised";
                        }
                        else
                        {
                            _ = SendApproveComplatedNotRealisedEmail(approvedChart.BIDComparisonId, root);
                            apiResult.Data = "not realised";
                        }


                        return apiResult;
                    }
                    catch (Exception ex)
                    {
                        apiResult.ErrorList.Add(ex.Message);
                        return apiResult;
                    }
                }
                else
                {
                    approvedChart.Stage = approvedChart.Stage += 1;
                    approvedChart.StatusId = (byte)Statuses.Open;
                    switch (approvedChart.Stage)
                    {
                        case 2:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage2;
                            break;
                        case 3:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage3;
                            break;
                        case 4:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage4;
                            break;
                        case 5:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage5;
                            break;
                        case 6:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage6;
                            break;
                        case 7:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage7;
                            break;
                        case 8:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage8;
                            break;
                        case 9:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage9;
                            break;
                        case 10:
                            approvedChart.ApproveStatusId = (byte)ApproveStatuses.Stage10;
                            break;
                    }
                    var comparisonBidReferances = await context.BIDReferances.Where(m => m.BIDComparisonId == approvedChart.BIDComparisonId).ToListAsync();
                    foreach (var bidRef in comparisonBidReferances)
                    {
                        bidRef.ApproveStatusId = approvedChart.ApproveStatusId;
                        bidRef.StatusId = (byte)Statuses.Open;
                        context.BIDReferances.Update(bidRef);
                    }
                    try
                    {
                        await context.SaveChangesAsync();
                        apiResult.OperationIsSuccess = true;
                        _ = SendApproveEmailRequest(approvedChart.BIDComparisonId, root);
                        return apiResult;
                    }
                    catch (Exception ex)
                    {
                        apiResult.ErrorList.Add(ex.Message);
                        return apiResult;
                    }
                }
            }

        }

        async Task<string> WonnedBidsSendSiteLineAsync(string siteId, List<int> wonnedBidIds)
        {
            var PONum = " ";
            try
            {
                var siteDatabase = await new SiteLogic().GetSiteDatabase(siteId);

                using (SqlConnection sqlConn = TransactionConfig.AppDbContextManualConnection)
                {
                    sqlConn.Open();

                    for (int i = 0; i < wonnedBidIds.Count; i++)
                    {
                        int wonnedBidId = wonnedBidIds[i];
                        var dt = new DataTable();
                        using (SqlCommand sqlCmd = new SqlCommand("[dbo].[RUS_CreatePO]", sqlConn))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@site_ref", siteDatabase == "QQZ" ? "QQZ" : "SOCARSTP");
                            sqlCmd.Parameters.AddWithValue("@BIdReferenceId", wonnedBidId);
                            dt.Load(sqlCmd.ExecuteReader());
                        }
                        PONum = dt.Rows[0][0] == DBNull.Value ? " " : dt.Rows[0][0].ToString();
                        using (var context = TransactionConfig.AppDbContext)
                        {
                            var bidEntity = await context.BIDReferances.FindAsync(wonnedBidId);
                            bidEntity.PONumber = PONum;
                            context.BIDReferances.Update(bidEntity);
                            await context.SaveChangesAsync();
                        }
                        dt = null;
                    }
                }
                return PONum;

            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                return " ";
            }

        }
        public async Task<ApiResult> Reject(ComparisonChartRejectHoldModel rejectModel, string userId, string root)
        {
            var apiResult = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                var comparisonChart = await context.ComparisonCharts
                    .Include(m => m.ApproveStage)
                    .Include(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                    .Include(m => m.BIDComparison)
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDReferances)
                    .Where(m => m.Id == rejectModel.ComparisonChartId)
                    .FirstOrDefaultAsync();
                comparisonChart.EditedDate = DateTime.Now;

                comparisonChart.ApproveStatusId = (byte)ApproveStatuses.Rejected;
                comparisonChart.StatusId = (byte)Statuses.Canceled;
                comparisonChart.Stage = 98;

                foreach (var bidRef in comparisonChart.BIDComparison.BIDReferances)
                {
                    bidRef.ApproveStatusId = (byte)ApproveStatuses.Rejected;
                    bidRef.StatusId = (byte)Statuses.Canceled;
                }

                comparisonChart.ComparisonChartReject = new ComparisonChartReject
                {
                    ComparisonChartId = comparisonChart.Id,
                    RejectDate = DateTime.Now,
                    RejectedStageDetailId = comparisonChart.ApproveStage.ApproveStageDetails.Where(m => m.Sequence == rejectModel.Stage).Select(m => m.Id).First(),
                    UserId = userId,
                    RejectReason = rejectModel.ModalValue
                };

                context.ComparisonCharts.Update(comparisonChart);
                context.SaveChanges();
                apiResult.OperationIsSuccess = true;
                _ = SendComparisonChartRejectedEmail(comparisonId: comparisonChart.BIDComparisonId, root: root);
            }

            return apiResult;
        }
        public async Task<ApiResult> Hold(ComparisonChartRejectHoldModel holdModel, string userId, string root)
        {
            var apiResult = new ApiResult();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var comparisonChart = await context.ComparisonCharts
                        .Include(m => m.ApproveStage)
                        .Include(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.BIDComparison)
                        .Include(m => m.BIDComparison).ThenInclude(m => m.BIDReferances)
                        .Where(m => m.Id == holdModel.ComparisonChartId)
                        .FirstOrDefaultAsync();
                    comparisonChart.EditedDate = DateTime.Now;

                    comparisonChart.StatusId = (byte)Statuses.OnHold;

                    foreach (var bidRef in comparisonChart.BIDComparison.BIDReferances)
                    {
                        bidRef.StatusId = (byte)Statuses.OnHold;
                    }

                    comparisonChart.ComparisonChartHolds.Add(new ComparisonChartHold
                    {
                        ComparisonChartId = comparisonChart.Id,
                        HoldDate = DateTime.Now,
                        ApproveStageDetailId = comparisonChart.ApproveStage.ApproveStageDetails.Where(m => m.Sequence == holdModel.Stage).Select(m => m.Id).First(),
                        UserId = userId,
                        HoldReason = holdModel.ModalValue
                    });
                    context.ComparisonCharts.Update(comparisonChart);
                    context.SaveChanges();
                    apiResult.OperationIsSuccess = true;
                    _ = SendComparisonChartHoldedEmail(comparisonId: comparisonChart.BIDComparisonId, root: root);
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                return apiResult;

            }

        }
        public async Task<List<AttachmentModel>> GetBidAttachments(int entityId, bool isComparison)
        {
            var result = new List<AttachmentModel>();//Check Olunacaq
            using (var context = TransactionConfig.AppDbContext)
            {

                var bidReferances = await context.BIDReferances
                                    .Include(m => m.Atachments)
                                    .Where(m => isComparison ? m.BIDComparisonId == entityId : m.Id == entityId)
                                    .ToListAsync();

                foreach (var bidRef in bidReferances)
                {
                    var attModel = new AttachmentModel();
                    attModel.BidComparisonNumber = bidRef.BIDNumber;
                    foreach (var bidAttach in bidRef.Atachments)
                    {
                        var attachModel = new BIDAttachmentDto
                        {
                            FileName = bidAttach.FileName,
                            TempBase64 = FileExtensions.ConvertFileToBase64(bidAttach.FilePath, "appfiles", bidAttach.FileBaseType),
                        };
                        attModel.Attachments.Add(attachModel);
                    }
                    result.Add(attModel);
                }

            }
            return result;
        }
        public async Task<ComparisonChartGettingDatas> GetComparisonChartDatasByTabIndex(int comparisonId, byte tabIndex, string userId)
        {
            switch (tabIndex)
            {
                case 1: //Waiting For Approve
                    return await new ComparisonChartLogic().GetComparisonChartApproveDatasAsync(comparisonId, userId);
                case 2: //All Comparison
                    return await new ComparisonChartLogic().GetComparisonChartAllComparisonDatas(comparisonId, userId);
                case 3: //Rejecteds
                    return await new ComparisonChartLogic().GetComparisonChartAllComparisonDatas(comparisonId, userId);
                case 5: //My Charts
                    return await new ComparisonChartLogic().GetComparisonChartAllComparisonDatas(comparisonId, userId);
            }
            return null;
        }
        public async Task<ApiResult> Resend(int comparisonId, string root)
        {
            var apiResult = new ApiResult();
            var resendEmailResult = await SendApproveEmailRequest(comparisonId, root);
            apiResult.OperationIsSuccess = resendEmailResult;
            apiResult.ErrorList.Add(resendEmailResult ? "" : "Operation Failed");
            return apiResult;
        }
        public async Task<ApiResult> Revise(int comparisonChartId)
        {
            var apiResult = new ApiResult();
            using (var context = TransactionConfig.AppDbContext)
            {
                try
                {
                    var compId = await context.ComparisonCharts.Where(m => m.Id == comparisonChartId).Select(m => m.BIDComparisonId).FirstOrDefaultAsync();
                    var comparison = await context.BIDComparisons
                  .Include(m => m.ComparisonChart)
                  .Include(m => m.BIDReferances)
                  .Where(m => m.Id == compId)
                  .FirstOrDefaultAsync();
                    comparison.ReviseNumber = comparison.ReviseNumber + 1;
                    foreach (var bidRef in comparison.BIDReferances)
                    {
                        bidRef.ApproveStatusId = (byte)ApproveStatuses.NotApproved;
                        bidRef.StatusId = (byte)Statuses.Draft;
                        bidRef.WonStatusId = (byte)WonStatuses.InProgress;
                        bidRef.CEOApproveDateFormatted = null;
                    }
                    var removedComparisonChartBaseInfos = await context.ComparisonChartApprovalBaseInfos.Where(m => m.ComparisonChartId == comparison.ComparisonChart.Id).ToListAsync();
                    var removedComparisonChartApproveStages = await context.ComparisonChartApproveStages.Where(m => m.ComparisonChartId == comparison.ComparisonChart.Id).ToListAsync();
                    var removedComparisonChartSingleSources = await context.RELComparisonChartSingleSources.Where(m => m.ComparisonChartId == comparison.ComparisonChart.Id).ToListAsync();
                    var removedComparisonChart = await context.ComparisonCharts.Where(m => m.Id == comparison.ComparisonChart.Id).FirstOrDefaultAsync();
                    context.ComparisonChartApprovalBaseInfos.RemoveRange(removedComparisonChartBaseInfos);
                    context.ComparisonChartApproveStages.RemoveRange(removedComparisonChartApproveStages);
                    context.RELComparisonChartSingleSources.RemoveRange(removedComparisonChartSingleSources);
                    context.ComparisonCharts.Remove(removedComparisonChart);
                    context.BIDComparisons.Update(comparison);
                    await context.SaveChangesAsync();
                    apiResult.OperationIsSuccess = true;
                }
                catch (Exception ex)
                {
                    apiResult.ErrorList.Add(ex.Message);
                }
            }
            return apiResult;
        }
        public async Task<List<BidRefWonnedLines>> GetComparisonWonnedLines(int comparisonId)
        {
            var result = new List<BidRefWonnedLines>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var bidChart = await context.ComparisonCharts
                    .Include(m => m.ApproveStage)
                    .ThenInclude(m => m.ApproveStageDetails)
                    .Where(m => m.BIDComparisonId == comparisonId)
                    .FirstOrDefaultAsync();


                var wonnedBidComp = await context.BIDComparisons
                    .Include(m => m.BIDReferances.Where(br => br.ApproveStatusId == (byte)ApproveStatuses.Approved))
                    .Include(m => m.BIDReferances).ThenInclude(m => m.RequestItems)
                    .Where(m => m.Id == comparisonId).FirstOrDefaultAsync();

                var lastStage = bidChart.ApproveStage.ApproveStageDetails.Count();
                foreach (var bidRef in wonnedBidComp.BIDReferances)
                {
                    var wonnedLines = await context.ComparisonChartApproveStages.Where(m => m.Stage == lastStage && m.BidReferanceId == bidRef.Id && m.ComparisonChartId == bidChart.Id).Select(m => m.BidReferanceItemRowPointer).ToListAsync();
                    result.Add(new BidRefWonnedLines
                    {
                        BidItems = await context.RELComparisonRequestItems.Include(m => m.BIDReferance).Where(m => m.BIDReferanceId == bidRef.Id && wonnedLines.Contains(m.RowPointer)).ToListAsync(),
                        BIDReferance = bidRef
                    });
                }
            }
            return result;
        }
        public async Task<UsdAznDiscount> GetChartWonnedTotals(int comparisonId, string siteId)
        {
            var result = new UsdAznDiscount();
            var site = await new SiteLogic().GetSite(siteId);
            var wonnedBids = await GetComparisonWonnedLines(comparisonId);

            foreach (var wonnedBid in wonnedBids)
            {
                result.Discount = (new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(wonnedBid.BIDReferance.Currency, wonnedBid.BIDReferance.DiscountValue, wonnedBid.BIDReferance.ComparisonDate.ToString("dd.MM.yyyy"), site.SiteName))._USD;
                foreach (var wonnedLine in wonnedBid.BidItems)
                {
                    var convertedDiscountPrice = wonnedBid.BIDReferance.DiscountTypeId > 1 ? 0 : (new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(wonnedBid.BIDReferance.Currency, wonnedLine.Discount, DateTime.Now.ToString(), site.SiteName))._USD;
                    var convertedTotalPrice = new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(wonnedBid.BIDReferance.Currency, wonnedLine.TotalPrice, wonnedBid.BIDReferance.ComparisonDate.ToString("dd.MM.yyyy"), site.SiteName);
                    result.TotalAzn = result.TotalAzn + convertedTotalPrice._AZN;
                    result.TotalUsd = result.TotalUsd + convertedTotalPrice._USD;
                    result.Discount = result.Discount + convertedDiscountPrice;
                }
            }
            return result;
        }
        //Email Operations
        public async Task<bool> SendApproveEmailRequest(int comparisonId, string root)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var stage = resultComparison.ComparisonChart.Stage;
                    var currentApprovals = resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.Where(m => m.Sequence == stage).FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);
                    var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                        .Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                        .Select(m => m.ApproveRole)
                        .SelectMany(m => m.GroupApproveRoles)
                        .Select(m => m.AppRoleId).Distinct().ToList();
                    var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                    sendedUsers = (await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList();
                    var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                    foreach (var user in sendedUsers)
                    {
                        MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart Approvals - Comparison № {resultComparison.ComparisonNumber}");
                        mail.To.Add(user.Email.Trim());
                        mail.Subject = $"BID Comparison Chart Approvals - Comparison № {resultComparison.ComparisonNumber}";
                        mail.IsBodyHtml = true;
                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartApproveIsPending.html")))
                        { body = reader.ReadToEnd(); }
                        body = body.Replace("{waitingforapprovalurl}", $"{dataBase}bidcomparison/bidchartcomparisons");
                        body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/1");
                        body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                        body = body.Replace("{fullname}", user.FirstName.Trim() + " " + user.LastName.Trim());
                        body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared.Trim());
                        body = body.Replace("{requester}", _comparisonReferance.Requester.Trim());
                        body = body.Replace("{destination}", _comparisonReferance.Destination.Trim());
                        mail.Body = body;

                        using (var sc = new SmtpClient())
                        {
                            sc.Port = 587;
                            sc.Host = "mail.apertech.net";
                            sc.EnableSsl = true;
                            sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                            sc.Send(mail);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> SendApproveComplatedEmail(int comparisonId, string root, string PONumber)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.ComparisonChart.CreatedUser)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    sendedUsers.Add(resultComparison.ComparisonChart.CreatedUser);
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);
                    foreach (var approval in resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.ToList())
                    {
                        var currentApprovals = approval;
                        var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                            //.Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                            .Select(m => m.ApproveRole)
                            .SelectMany(m => m.GroupApproveRoles)
                            .Select(m => m.AppRoleId).Distinct().ToList();
                        var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                        sendedUsers.AddRange((await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList());

                    }
                    sendedUsers = sendedUsers.CustomDistinctBy(m => m.Id).ToList();
                    foreach (var user in sendedUsers)
                    {
                        if (user.Email.Trim() != "f.aghayev@socar-stp.az")
                        {
                            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart Approved - Comparison № {resultComparison.ComparisonNumber}");
                            mail.To.Add(user.Email.Trim());
                            mail.Subject = $"BID Comparison Chart Approved - Comparison № {resultComparison.ComparisonNumber}";
                            mail.IsBodyHtml = true;
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartApproved.html")))
                            { body = reader.ReadToEnd(); }
                            body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/2");
                            body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                            body = body.Replace("{poreference}", PONumber);
                            body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared);
                            mail.Body = body;

                            using (var sc = new SmtpClient())
                            {
                                sc.Port = 587;
                                sc.Host = "mail.apertech.net";
                                sc.EnableSsl = true;
                                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                                sc.Send(mail);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> SendRealiseToSyteLineComplatedEmail(int comparisonId, string root, string PONumber)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.ComparisonChart.CreatedUser)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    sendedUsers.Add(resultComparison.ComparisonChart.CreatedUser);
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);
                    foreach (var approval in resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.ToList())
                    {
                        var currentApprovals = approval;
                        var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                            //.Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                            .Select(m => m.ApproveRole)
                            .SelectMany(m => m.GroupApproveRoles)
                            .Select(m => m.AppRoleId).Distinct().ToList();
                        var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                        sendedUsers.AddRange((await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList());

                    }
                    sendedUsers = sendedUsers.CustomDistinctBy(m => m.Id).ToList();
                    foreach (var user in sendedUsers)
                    {
                        if (user.Email.Trim() != "f.aghayev@socar-stp.az")
                        {
                            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("socarstpinfo@apertech.net", $"Out of Deadline Comparison Chart is Sent to Syteline - Comparison № {resultComparison.ComparisonNumber}");
                            mail.To.Add(user.Email.Trim());
                            mail.Subject = $"Out of Deadline Comparison Chart is Sent to Syteline - Comparison № {resultComparison.ComparisonNumber}";
                            mail.IsBodyHtml = true;
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "RealiseToSyteline.html")))
                            { body = reader.ReadToEnd(); }
                            body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/2");
                            body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                            body = body.Replace("{poreference}", PONumber);
                            body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared);
                            mail.Body = body;

                            using (var sc = new SmtpClient())
                            {
                                sc.Port = 587;
                                sc.Host = "mail.apertech.net";
                                sc.EnableSsl = true;
                                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                                sc.Send(mail);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> SendApproveComplatedNotRealisedEmail(int comparisonId, string root)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.ComparisonChart.CreatedUser)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    sendedUsers.Add(resultComparison.ComparisonChart.CreatedUser);
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);
                    foreach (var approval in resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.ToList())
                    {
                        var currentApprovals = approval;
                        var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                            //.Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                            .Select(m => m.ApproveRole)
                            .SelectMany(m => m.GroupApproveRoles)
                            .Select(m => m.AppRoleId).Distinct().ToList();
                        var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                        sendedUsers.AddRange((await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList());

                    }
                    sendedUsers = sendedUsers.CustomDistinctBy(m => m.Id).ToList();
                    foreach (var user in sendedUsers)
                    {
                        if (user.Email.Trim() != "f.aghayev@socar-stp.az")
                        {
                            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart Approved,but not send to Syteline - Comparison № {resultComparison.ComparisonNumber}");
                            mail.To.Add(user.Email.Trim());
                            mail.Subject = $"BID Comparison Chart Approved,but not send to Syteline - Comparison № {resultComparison.ComparisonNumber}";
                            mail.IsBodyHtml = true;
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartApprovedNotRealised.html")))
                            { body = reader.ReadToEnd(); }
                            body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/2");
                            body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                            body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared);
                            mail.Body = body;

                            using (var sc = new SmtpClient())
                            {
                                sc.Port = 587;
                                sc.Host = "mail.apertech.net";
                                sc.EnableSsl = true;
                                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                                sc.Send(mail);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> SendComparisonChartRejectedEmail(int comparisonId, string root)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.ComparisonChart.CreatedUser)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartReject)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartReject).ThenInclude(m => m.RejectedStageDetail)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    sendedUsers.Add(resultComparison.ComparisonChart.CreatedUser);
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);
                    int stage = resultComparison.ComparisonChart.Stage == 1 ? 1 : resultComparison.ComparisonChart.Stage - 1;
                    var lastApprovedUsers = resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.Take(stage).ToList();
                    foreach (var approval in lastApprovedUsers)
                    {
                        var currentApprovals = approval;
                        var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                            //.Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                            .Select(m => m.ApproveRole)
                            .SelectMany(m => m.GroupApproveRoles)
                            .Select(m => m.AppRoleId).Distinct().ToList();
                        var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                        sendedUsers.AddRange((await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList());

                    }
                    sendedUsers = sendedUsers.CustomDistinctBy(m => m.Id).ToList();
                    var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;
                    foreach (var user in sendedUsers)
                    {
                        if (user.Email.Trim() != "f.aghayev@socar-stp.az")
                        {
                            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart Rejected - Comparison № {resultComparison.ComparisonNumber}");
                            mail.To.Add(user.Email.Trim());
                            mail.Subject = $"BID Comparison Chart Rejected - Comparison № {resultComparison.ComparisonNumber}";
                            mail.IsBodyHtml = true;
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartRejected.html")))
                            { body = reader.ReadToEnd(); }
                            body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/2");
                            body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                            body = body.Replace("{rejectreason}", resultComparison.ComparisonChart.ComparisonChartReject.RejectReason);
                            body = body.Replace("{approvestagedetailname}", resultComparison.ComparisonChart.ComparisonChartReject.RejectedStageDetail.ApproveStageDetailName);
                            body = body.Replace("{rejectedfullname}", resultComparison.ComparisonChart.ComparisonChartReject.User.FirstName + " " + resultComparison.ComparisonChart.ComparisonChartReject.User.LastName);
                            body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared);
                            mail.Body = body;

                            using (var sc = new SmtpClient())
                            {
                                sc.Port = 587;
                                sc.Host = "mail.apertech.net";
                                sc.EnableSsl = true;
                                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                                sc.Send(mail);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> SendComparisonChartHoldedEmail(int comparisonId, string root)
        {
            var sendedUsers = new List<AppUser>();
            try
            {
                var dataBase = Statics.IsProductionEnvironment ? Statics.API_SOCAR_BASE : Statics.API_BASE;

                using (var context = TransactionConfig.AppDbContext)
                {
                    var resultComparison = await context.BIDComparisons
                        .Include(m => m.ComparisonChart.CreatedUser)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartReject)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartReject).ThenInclude(m => m.RejectedStageDetail)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartHolds).ThenInclude(m => m.ApproveStageDetail)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ComparisonChartHolds).ThenInclude(m => m.User)
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails).ThenInclude(m => m.ApproveRole).ThenInclude(m => m.GroupApproveRoles)
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails).ThenInclude(m => m.ApproveRoleApproveStageDetails)
                        .Where(m => m.Id == comparisonId).FirstAsync();
                    var resultComparisonChartCurrentHold = resultComparison.ComparisonChart.ComparisonChartHolds.Last();
                    sendedUsers.Add(resultComparison.ComparisonChart.CreatedUser);
                    var _comparisonReferance = resultComparison.BIDReferances.FirstOrDefault();
                    var maxBidReferanceValue = resultComparison.BIDReferances.Max(m => m.TotalAmount);

                    int stage = resultComparison.ComparisonChart.Stage == 1 ? 1 : resultComparison.ComparisonChart.Stage - 1;
                    var lastApprovedUsers = resultComparison.ComparisonChart.ApproveStage.ApproveStageDetails.Take(stage).ToList();
                    foreach (var approval in lastApprovedUsers)
                    {
                        var currentApprovals = approval;
                        var selectedGroups = currentApprovals.ApproveRoleApproveStageDetails
                            //.Where(m => m.AmountFrom <= maxBidReferanceValue && m.AmountTo >= maxBidReferanceValue)
                            .Select(m => m.ApproveRole)
                            .SelectMany(m => m.GroupApproveRoles)
                            .Select(m => m.AppRoleId).Distinct().ToList();
                        var approvalUserIds = await context.UserRoles.Where(m => selectedGroups.Contains(m.RoleId)).Select(m => m.UserId).ToListAsync();
                        sendedUsers.AddRange((await context.Users.Where(m => approvalUserIds.Contains(m.Id)).ToListAsync()).Distinct().ToList());

                    }
                    sendedUsers = sendedUsers.CustomDistinctBy(m => m.Id).ToList();
                    foreach (var user in sendedUsers)
                    {
                        if (user.Email.Trim() != "f.aghayev@socar-stp.az")
                        {
                            MailAddress address = new MailAddress("socarstpinfo@apertech.net");
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("socarstpinfo@apertech.net", $"BID Comparison Chart Holded - Comparison № {resultComparison.ComparisonNumber}");
                            mail.To.Add(user.Email.Trim());
                            mail.Subject = $"BID Comparison Chart Holded - Comparison № {resultComparison.ComparisonNumber}";
                            mail.IsBodyHtml = true;
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(root, "templates", "ChartHolded.html")))
                            { body = reader.ReadToEnd(); }
                            body = body.Replace("{comparisonurl}", $"{dataBase}bidcomparison/comparisonchart/{resultComparison.Id}/2");
                            body = body.Replace("{comparisonnumber}", resultComparison.ComparisonNumber);
                            body = body.Replace("{holdreason}", resultComparisonChartCurrentHold.HoldReason);
                            body = body.Replace("{approvestagedetailname}", resultComparisonChartCurrentHold.ApproveStageDetail.ApproveStageDetailName);
                            body = body.Replace("{holdedfullname}", resultComparisonChartCurrentHold.User.FirstName + " " + resultComparisonChartCurrentHold.User.LastName);
                            body = body.Replace("{buyer}", resultComparison.ComparisonChartPrepared);
                            mail.Body = body;

                            using (var sc = new SmtpClient())
                            {
                                sc.Port = 587;
                                sc.Host = "mail.apertech.net";
                                sc.EnableSsl = true;
                                sc.Credentials = new NetworkCredential("socarstpinfo@apertech.net", "Toshiba.509.");
                                sc.Send(mail);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        //Excel Operations
        public async Task<ExportModel> ExportComparisonChart(int comparisonId, string userId, string root, string siteId)
        {
            var comparisonDatas = await GetComparisonChartAllComparisonDatas(comparisonId, userId);
            byte[] comparisonChartExcelFileBytes = await GetComparisonChartTemplateDatas(root);
            string generetedExcelFilePath = GenereteTempComparisonChartTemplate(root, comparisonChartExcelFileBytes);
            var wonDatas = await GetChartWonnedTotals(comparisonId, siteId);

            using (var workbook = new XLWorkbook(generetedExcelFilePath))
            {
                var workSheet = GetCompatibleWorkSheet(workbook, comparisonDatas);

                if (comparisonDatas.BidReferanceInformations.Count == 1)
                    SetSingleSurceExcelDatas(workSheet, comparisonDatas, root, wonDatas);
                else
                    SetMultipleSourceExcelDatas(workSheet, comparisonDatas, root, wonDatas);
                ExcelOptions(workbook, workSheet);
            }
            var generetedExcelFileBytes = await File.ReadAllBytesAsync(generetedExcelFilePath);

            return ReturnModel(comparisonDatas, generetedExcelFilePath, generetedExcelFileBytes);

            #region Local Functions
            void ExcelOptions(XLWorkbook workbook, IXLWorksheet workSheet)
            {
                //workSheet.Protect(password: "12345").AllowNone();
                workbook.Save();
            }

            string GenereteTempComparisonChartTemplate(string root, byte[] comparisonChartExcelFileBytes)
            {
                var generetedExcelFilePath = Path.Combine(root, "exceltemplates", $"{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}ComparisonChart.xlsx");
                File.WriteAllBytes(generetedExcelFilePath, comparisonChartExcelFileBytes);
                return generetedExcelFilePath;
            }

            async Task<byte[]> GetComparisonChartTemplateDatas(string root)
            {
                var comparisonChartExcelFilePath = Path.Combine(root, "templates", "ComparisonChart.xlsx");
                var comparisonChartExcelFileBytes = await File.ReadAllBytesAsync(comparisonChartExcelFilePath);
                return comparisonChartExcelFileBytes;
            }

            ExportModel ReturnModel(ComparisonChartGettingDatas comparisonDatas, string generetedExcelFilePath, byte[] generetedExcelFileBytes)
            {
                return new ExportModel
                {
                    FileBase64 = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + Convert.ToBase64String(generetedExcelFileBytes, 0, generetedExcelFileBytes.Length),
                    FileName = $"Comparison_Chart_{comparisonDatas.ComparisonNumber}_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}"
                };
            }

            IXLWorksheet GetCompatibleWorkSheet(XLWorkbook workbook, ComparisonChartGettingDatas comparisonDatas)
            {
                int workSheetCount = workbook.Worksheets.Count();
                workbook.Worksheet(comparisonDatas.BidReferanceInformations.Count).Position = 1;
                for (int i = 2; i <= workSheetCount; i++)
                {
                    workbook.Worksheet(2).Delete();
                }
                workbook.Worksheet(1).Name = String.IsNullOrEmpty(comparisonDatas.Destination) ? "Sheet" : comparisonDatas.Destination;
                return workbook.Worksheet(1);
            }

            void SetSingleSurceExcelDatas(IXLWorksheet workSheet, ComparisonChartGettingDatas comparisonDatas, string root, UsdAznDiscount wonnedLines)
            {
                //Approved Total Amounts Start
                for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                {
                    var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                    switch (i + 1)
                    {
                        case 1:
                            workSheet.AddValue(2, 18, appStgDtl.ApproveStageDetailName);
                            workSheet.AddValue(2, 20, appStgDtl.TotalApprovedAmount, 3);
                            workSheet.AddValue(12, 19, appStgDtl.ApproveStageDetailName);
                            break;
                        case 2:
                            workSheet.AddValue(3, 18, appStgDtl.ApproveStageDetailName);
                            workSheet.AddValue(3, 20, appStgDtl.TotalApprovedAmount, 3);
                            workSheet.AddValue(12, 20, appStgDtl.ApproveStageDetailName);
                            break;
                        case 3:
                            workSheet.AddValue(4, 18, appStgDtl.ApproveStageDetailName);
                            workSheet.AddValue(4, 20, appStgDtl.TotalApprovedAmount, 3);
                            workSheet.AddValue(12, 21, appStgDtl.ApproveStageDetailName);
                            break;
                        case 4:
                            workSheet.AddValue(5, 18, appStgDtl.ApproveStageDetailName);
                            workSheet.AddValue(5, 20, appStgDtl.TotalApprovedAmount, 3);
                            workSheet.AddValue(12, 22, appStgDtl.ApproveStageDetailName);
                            break;
                        case 5:
                            workSheet.AddValue(6, 18, appStgDtl.ApproveStageDetailName);
                            workSheet.AddValue(6, 20, appStgDtl.TotalApprovedAmount, 3);
                            workSheet.AddValue(12, 23, appStgDtl.ApproveStageDetailName);
                            break;
                    }

                }
                //Approved Total Amounts Start

                //Header Informations Start
                workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                workSheet.AddValue(8, 5, $"Request Number: {comparisonDatas.RequestNo}");
                workSheet.AddValue(8, 6, $"Destitation: {comparisonDatas.Destination}");
                workSheet.AddValue(8, 7, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                workSheet.AddValue(8, 11, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                workSheet.AddValue(8, 13, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                workSheet.AddValue(8, 16, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                workSheet.AddValue(8, 20, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                //Header Informations End

                //Delivery Terms => Total Price In USD Start
                var singleBidRef = comparisonDatas.BidReferanceInformations[0];
                workSheet.AddValue(14, 14, singleBidRef.DeliveryTerms);
                workSheet.AddValue(15, 14, singleBidRef.DeliveryTime);
                workSheet.AddValue(16, 14, singleBidRef.PaymentTerms);
                workSheet.AddValue(17, 14, singleBidRef.Currency);
                workSheet.AddValue(17, 16, singleBidRef.TotalAmount);
                workSheet.AddValue(18, 16, singleBidRef.TotalAZN);
                workSheet.AddValue(19, 16, singleBidRef.TotalUSD);
                workSheet.AddValue(19, 19, singleBidRef.DiscountPrice);
                //Delivery Terms => Total Price In USD End

                //Comments,Totals and Single Source Reason Start
                workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                {
                    var bidComments = comparisonDatas.ApprovalStageDetails;
                    workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                    workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                }
                //Totals

                workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace());
                workSheet.AddValue(26, 13, wonDatas.TotalUsd.FormatDecimalWithSpace());
                workSheet.AddValue(26, 16, wonDatas.Discount.FormatDecimalWithSpace());
                //Single Source Reason
                workSheet.AddValue(27, 11, comparisonDatas.SingleSourceReason);
                //Comments,Totals and Single Source Reason End

                //Approvals Start
                for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                {
                    var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                    workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                    workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                    workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                    workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                    if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                    {
                        var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                        var signature = workSheet.AddPicture(signaturePath)
                            .MoveTo(workSheet.Cell(30 + i + 1, 12))
                            .Scale(0.02);
                    }
                }
                //Approvals End

                //Item Operations Start
                if (comparisonDatas.RequestInformations.Count > 1)
                    workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                {
                    var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                    workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                    workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                    workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                    workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                    workSheet.AddValue(i, 11, currReqInfo.Budget);
                    workSheet.AddValue(i, 12, "0");
                    workSheet.AddValue(i, 13, "0");
                    var currBidRefItem = comparisonDatas.BidReferanceInformations[0].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                    workSheet.AddValue(i, 14, currBidRefItem.BidQuantity.ToString());
                    workSheet.AddValue(i, 15, currBidRefItem.UnitPrice);
                    workSheet.AddValue(i, 16, currBidRefItem.TotalPrice);
                    workSheet.AddValue(i, 17, currBidRefItem.Discount);
                    workSheet.AddValue(i, 18, currBidRefItem.BidComment);
                    workSheet.AddValue(10, 14, comparisonDatas.BidReferanceInformations[0].BIDReferanceNumber);
                    workSheet.AddValue(11, 14, comparisonDatas.BidReferanceInformations[0].VendorName);
                    for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                    {
                        var currASD = currBidRefItem.ApprovalStageDetails[asd];
                        switch (asd + 1)
                        {
                            case 1:
                                workSheet.AddValue(i, 19, currASD.IsApproved ? "ü" : "");
                                break;
                            case 2:
                                workSheet.AddValue(i, 20, currASD.IsApproved ? "ü" : "");
                                break;
                            case 3:
                                workSheet.AddValue(i, 21, currASD.IsApproved ? "ü" : "");
                                break;
                            case 4:
                                workSheet.AddValue(i, 22, currASD.IsApproved ? "ü" : "");
                                break;
                            case 5:
                                workSheet.AddValue(i, 23, currASD.IsApproved ? "ü" : "");
                                break;

                        }
                    }
                }
                //Item Operations End
            }

            void SetMultipleSourceExcelDatas(IXLWorksheet workSheet, ComparisonChartGettingDatas comparisonDatas, string root, UsdAznDiscount wonnedLines)
            {
                var bidRefCount = comparisonDatas.BidReferanceInformations.Count();
                switch (bidRefCount)
                {
                    case 2:
                        BidRefOperations2();
                        break;
                    case 3:
                        BidRefOperations3();
                        break;
                    case 4:
                        BidRefOperations4();
                        break;
                    case 5:
                        BidRefOperations5();
                        break;
                    case 6:
                        BidRefOperations6();
                        break;
                    case 7:
                        BidRefOperations7();
                        break;
                    case 8:
                        BidRefOperations8();
                        break;
                    case 9:
                        BidRefOperations9();
                        break;
                    case 10:
                        BidRefOperations10();
                        break;
                    case 11:
                        BidRefOperations11();
                        break;
                    case 12:
                        BidRefOperations12();
                        break;
                    case 13:
                        BidRefOperations13();
                        break;
                    case 14:
                        BidRefOperations14();
                        break;
                    case 15:
                        BidRefOperations15();
                        break;
                }


                void BidRefOperations2()
                {
                    //Approved Total Amounts Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 27, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 29, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 27, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 29, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 27, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 29, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 27, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 29, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 27, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 29, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 11, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 13, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 15, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 19, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 26, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 13, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations3()
                {
                    //Approved Total Amounts Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 37, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 39, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 37, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 39, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 37, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 39, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 37, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 39, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 37, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 39, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        //if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        //{
                        //    var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                        //    var signature = workSheet.AddPicture(signaturePath)
                        //        .MoveTo(workSheet.Cell(30 + i + 1, 12))
                        //        .Scale(0.02);
                        //}
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations4()
                {
                    //Approved Total Amounts Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations5()
                {
                    //Approved Total Amounts Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations6()
                {
                    //Approved Total Amounts Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations7()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations8()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations9()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations10()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations11()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations12()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations13()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations14()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
                void BidRefOperations15()
                {
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        switch (i + 1)
                        {
                            case 1:
                                workSheet.AddValue(2, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(2, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 2:
                                workSheet.AddValue(3, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(3, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 3:
                                workSheet.AddValue(4, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(4, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 4:
                                workSheet.AddValue(5, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(5, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                            case 5:
                                workSheet.AddValue(6, 47, appStgDtl.ApproveStageDetailName);
                                workSheet.AddValue(6, 49, appStgDtl.TotalApprovedAmount, 3);
                                break;
                        }

                    }
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var appStgDtl = comparisonDatas.ApprovalStageDetails[i];
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            switch (i + 1)
                            {
                                case 1:
                                    workSheet.AddValue(12, 19 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 2:
                                    workSheet.AddValue(12, 20 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 3:
                                    workSheet.AddValue(12, 21 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 4:
                                    workSheet.AddValue(12, 22 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                                case 5:
                                    workSheet.AddValue(12, 23 + (10 * bri), appStgDtl.ApproveStageDetailName);
                                    break;
                            }
                        }

                    }
                    //Approved Total Amounts Start

                    //Header Informations Start
                    workSheet.AddValue(8, 2, $"Entity: {comparisonDatas.Entity}");
                    workSheet.AddValue(8, 4, $"Requester: {comparisonDatas.Requester}");
                    workSheet.AddValue(8, 6, $"Request Number: {comparisonDatas.RequestNo}");
                    workSheet.AddValue(8, 8, $"Destination: {comparisonDatas.Destination}");
                    workSheet.AddValue(8, 13, $"Comparison Chart Prepared by: {comparisonDatas.Buyer}");
                    workSheet.AddValue(8, 16, $"Comparison Number: {comparisonDatas.ComparisonNumber}");
                    workSheet.AddValue(8, 24, $"Project Warehouse: {comparisonDatas.WarehouseName}");
                    workSheet.AddValue(8, 30, $"Comparison Date: {comparisonDatas.ComparisonDate}");
                    workSheet.AddValue(8, 36, $"Comparison Deadline: {comparisonDatas.ComparisonDeadline}");
                    //Header Informations End

                    //Delivery Terms => Total Price In USD Start
                    for (int i = 0; i < comparisonDatas.BidReferanceInformations.Count; i++)
                    {
                        var singleBidRef = comparisonDatas.BidReferanceInformations[i];
                        workSheet.AddValue(10, 14 + (10 * i), singleBidRef.BIDReferanceNumber);
                        workSheet.AddValue(11, 14 + (10 * i), singleBidRef.VendorName);
                        workSheet.AddValue(14, 14 + (10 * i), singleBidRef.DeliveryTerms);
                        workSheet.AddValue(15, 14 + (10 * i), singleBidRef.DeliveryTime);
                        workSheet.AddValue(16, 14 + (10 * i), singleBidRef.PaymentTerms);
                        workSheet.AddValue(17, 14 + (10 * i), singleBidRef.Currency);
                        workSheet.AddValue(17, 16 + (10 * i), singleBidRef.TotalAmount);
                        workSheet.AddValue(18, 16 + (10 * i), singleBidRef.TotalAZN);
                        workSheet.AddValue(19, 16 + (10 * i), singleBidRef.TotalUSD);
                        workSheet.AddValue(19, 19 + (10 * i), singleBidRef.DiscountPrice);
                    }
                    //Delivery Terms => Total Price In USD End

                    //Comments,Totals and Single Source Reason Start
                    workSheet.AddValue(20, 8, comparisonDatas.ComProcurementSpecialist);
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var bidComments = comparisonDatas.ApprovalStageDetails;
                        workSheet.AddValue(20 + i + 1, 8, bidComments[i].Comment);
                        workSheet.AddValue(20 + i + 1, 2, $"COMMENTS: {bidComments[i].ApproveStageDetailName}");
                    }
                    //Totals

                    workSheet.AddValue(26, 10, wonDatas.TotalAzn.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 14, wonDatas.TotalUsd.FormatDecimalWithSpace(), 1);
                    workSheet.AddValue(26, 26, wonDatas.Discount.FormatDecimalWithSpace());
                    //Single Source Reason
                    workSheet.AddValue(27, 14, comparisonDatas.WonnerVendorAndLines);
                    //Comments,Totals and Single Source Reason End

                    //Approvals Start
                    for (int i = 0; i < comparisonDatas.ApprovalStageDetails.Count; i++)
                    {
                        var approvalInfo = comparisonDatas.ApprovalStageDetails[i];
                        workSheet.AddValue(30 + i + 1, 2, (i + 1).ToString());
                        workSheet.AddValue(30 + i + 1, 3, approvalInfo.ApproveStageDetailName);
                        workSheet.AddValue(30 + i + 1, 6, approvalInfo.NameSurname);
                        workSheet.AddValue(30 + i + 1, 9, approvalInfo.ApproveDate);

                        if (!string.IsNullOrEmpty(approvalInfo.SignaturePath))
                        {
                            var signaturePath = Path.Combine(root, "appfiles", approvalInfo.SignaturePath);

                            var signature = workSheet.AddPicture(signaturePath)
                                .MoveTo(workSheet.Cell(30 + i + 1, 12))
                                .Scale(0.02);
                        }
                    }
                    //Approvals End

                    //Item Operations Start
                    if (comparisonDatas.RequestInformations.Count > 1)
                        workSheet.Row(13).InsertRowsBelow(comparisonDatas.RequestInformations.Count - 1);
                    for (int i = 13; i < comparisonDatas.RequestInformations.Count + 13; i++)
                    {

                        var currReqInfo = comparisonDatas.RequestInformations[i - 13];
                        workSheet.AddValue(i, 2, currReqInfo.PRItemNo.ToString());
                        workSheet.AddValue(i, 3, currReqInfo.DescriptionOfRequiredPurchase, 4);
                        workSheet.AddValue(i, 8, currReqInfo.RequestQuantity, 0);
                        workSheet.AddValue(i, 9, currReqInfo.UOM, 1);
                        workSheet.AddValue(i, 11, currReqInfo.Budget);
                        workSheet.AddValue(i, 12, "0");
                        workSheet.AddValue(i, 13, "0");
                        for (int bri = 0; bri < comparisonDatas.BidReferanceInformations.Count; bri++)
                        {
                            var currBidRefItem = comparisonDatas.BidReferanceInformations[bri].BIDInformationItems.Where(m => m.RowPointer == currReqInfo.RowPointer).First();
                            workSheet.AddValue(i, 14 + (10 * bri), currBidRefItem.BidQuantity.ToString());
                            workSheet.AddValue(i, 15 + (10 * bri), currBidRefItem.UnitPrice);
                            workSheet.AddValue(i, 16 + (10 * bri), currBidRefItem.TotalPrice);
                            workSheet.AddValue(i, 17 + (10 * bri), currBidRefItem.Discount);
                            workSheet.AddValue(i, 18 + (10 * bri), currBidRefItem.BidComment);
                            for (int asd = 0; asd < currBidRefItem.ApprovalStageDetails.Count; asd++)
                            {
                                var currASD = currBidRefItem.ApprovalStageDetails[asd];
                                switch (asd + 1)
                                {
                                    case 1:
                                        workSheet.AddValue(i, 19 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 2:
                                        workSheet.AddValue(i, 20 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 3:
                                        workSheet.AddValue(i, 21 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 4:
                                        workSheet.AddValue(i, 22 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;
                                    case 5:
                                        workSheet.AddValue(i, 23 + (10 * bri), currASD.IsApproved ? "ü" : "");
                                        break;

                                }
                            }
                        }

                    }
                    //Item Operations End
                }
            }
            #endregion
        }
    }
}
