using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.Report;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Domain.Models.AppDbContext;
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
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class ReportLogic
    {
        public async Task<List<NameValueModel>> GetBIDsStatusReport(DateTime dateFrom, DateTime dateTo, string userId, string siteId)
        {
            var result = new List<NameValueModel>();
            var site = await new SiteLogic().GetSite(siteId);
            var userGroupBuyers = await new ComparisonChartLogic().GetUserGroupBuyersByUserId(userId, site.SiteDatabase);
            var userGroupSiteWarehouses = await new ComparisonChartLogic().GetUserGroupSiteWarehouses(userId, site.Id);

            int draftCount = 0;
            int onHoldCount = 0;
            int openCount = 0;
            int cancledCount = 0;
            int closedCount = 0;

            using (var context = TransactionConfig.AppDbContext)
            {

                var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
                        .Include(m => m.ComparisonChart)
                        .Include(m => m.BIDReferances).ThenInclude(m => m.ApproveStatus)
                        .Where(m => userGroupBuyers.Contains(m.ComparisonChartPrepared) && m.ComparisonChart.CreatedDate >= dateFrom && m.ComparisonChart.CreatedDate <= dateTo ||
                        userGroupBuyers.Contains(m.ComparisonChartPrepared) && m.CreateDate >= dateFrom && m.CreateDate <= dateTo && m.ComparisonChart == null)
                        .ToListAsync();

                foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
                {
                    var _comparisonReferance = bidComparison.BIDReferances.FirstOrDefault();
                    if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && _comparisonReferance.SiteId == site.Id))
                    {
                        if (bidComparison.ComparisonChart == null)
                        {
                            draftCount++;
                        }
                        else
                        {
                            switch (_comparisonReferance.StatusId)
                            {
                                case (byte)Statuses.OnHold:
                                    onHoldCount++;
                                    break;
                                case (byte)Statuses.Open:
                                    openCount++;
                                    break;
                                case (byte)Statuses.Canceled:
                                    cancledCount++;
                                    break;
                                case (byte)Statuses.Closed:
                                    closedCount++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    _comparisonReferance = null;
                }
            }

            result.Add(new NameValueModel { Name = "Draft", Value = draftCount });
            result.Add(new NameValueModel { Name = "OnHold", Value = onHoldCount });
            result.Add(new NameValueModel { Name = "Open", Value = openCount });
            result.Add(new NameValueModel { Name = "Canceled", Value = cancledCount });
            result.Add(new NameValueModel { Name = "Closed", Value = closedCount });
            return result;
        }
        public async Task<List<NameValueModel>> GetSingleSourceReport(DateTime dateFrom, DateTime dateTo, string userId, string siteId)
        {
            var result = new List<NameValueModel>();
            var site = await new SiteLogic().GetSite(siteId);
            var acceptedComparisonResultList = new List<BIDComparison>();
            var userGroupBuyers = await new ComparisonChartLogic().GetUserGroupBuyersByUserId(userId, site.SiteDatabase);
            var userGroupSiteWarehouses = await new ComparisonChartLogic().GetUserGroupSiteWarehouses(userId, site.Id);
            using (var context = TransactionConfig.AppDbContext)
            {

                var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.SingleSourceReasons)
                        .Include(m => m.BIDReferances)
                        .Where(m => userGroupBuyers.Contains(m.ComparisonChartPrepared) && m.ComparisonChart.CreatedDate >= dateFrom && m.ComparisonChart.CreatedDate <= dateTo)
                        .ToListAsync();

                foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
                {
                    var _comparisonReferance = bidComparison.BIDReferances.FirstOrDefault();
                    if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && m.SiteId == site.Id))
                    {
                        acceptedComparisonResultList.Add(bidComparison);
                    }
                }
            }
            var singleSourceSum = acceptedComparisonResultList.Where(m => m.ComparisonChart.SingleSourceReasons.Any()).Count();
            var multipleSourceSum = acceptedComparisonResultList.Where(m => !m.ComparisonChart.SingleSourceReasons.Any()).Count();
            result.Add(new NameValueModel { Name = "Single Source", Value = singleSourceSum });
            result.Add(new NameValueModel { Name = "Competitive Bidding", Value = multipleSourceSum });
            return result;
        }


        public List<NameCountStatusModel> GetAverageProceedDurationReportReport(DateTime dateFrom, DateTime dateTo, string userId)
        {
            var result = new List<NameCountStatusModel>();
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection sqlConn = TransactionConfig.AppDbContextManualConnection)
                {
                    string spName = "[dbo].[SP_AverageStatus]";

                    using (SqlCommand sqlCmd = new SqlCommand(spName, sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserId", userId);
                        sqlCmd.Parameters.AddWithValue("@DateFrom", dateFrom);
                        sqlCmd.Parameters.AddWithValue("@DateTo", dateTo);
                        sqlConn.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result.Add(new NameCountStatusModel
                    {
                        Count = dt.Rows[i]["AVGDateDiff"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["AVGDateDiff"]),
                        Name = dt.Rows[i]["buyer"] == DBNull.Value ? " " : dt.Rows[i]["buyer"].ToString(),
                        Status = dt.Rows[i]["Status"] == DBNull.Value ? " " : dt.Rows[i]["Status"].ToString().ToLower(),
                    });
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }

        public List<NameCountStatusModel> GetComparisonDeadlineReportReport(DateTime dateFrom, DateTime dateTo, string userId)
        {
            var result = new List<NameCountStatusModel>();
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection sqlConn = TransactionConfig.AppDbContextManualConnection)
                {
                    string spName = "[dbo].[SP_AverageStatus]";

                    using (SqlCommand sqlCmd = new SqlCommand(spName, sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@UserId", userId);
                        sqlCmd.Parameters.AddWithValue("@DateFrom", dateFrom);
                        sqlCmd.Parameters.AddWithValue("@DateTo", dateTo);
                        sqlConn.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result.Add(new NameCountStatusModel
                    {
                        Count = dt.Rows[i]["AVGDateDiff"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["AVGDateDiff"]),
                        Name = dt.Rows[i]["buyer"] == DBNull.Value ? " " : dt.Rows[i]["buyer"].ToString(),
                        Status = dt.Rows[i]["Status"] == DBNull.Value ? " " : dt.Rows[i]["Status"].ToString().ToLower(),
                    });
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }

        public async Task<List<NameDiscountTotal>> GetDiscountReport(DateTime dateFrom, DateTime dateTo, string userId, string siteId)
        {
            var result = new List<NameDiscountTotal>();
            try
            {
                var site = await new SiteLogic().GetSite(siteId);
                var userGroupBuyers = await new ComparisonChartLogic().GetUserGroupBuyersByUserId(userId, site.SiteDatabase);
                var userGroupSiteWarehouses = await new ComparisonChartLogic().GetUserGroupSiteWarehouses(userId, site.Id);
                using (var context = TransactionConfig.AppDbContext)
                {
                    var wonnedComparisons = await GetWonnedComparisons(site, userGroupBuyers, userGroupSiteWarehouses, context);
                    foreach (var wonnedComp in wonnedComparisons)
                    {
                        var comparisonChartLastStage = wonnedComp.ComparisonChart.ApproveStage.ApproveStageDetails.Count();
                        foreach (var wonnedBidRef in wonnedComp.BIDReferances)
                        {
                            var wonnedLines = await context.ComparisonChartApproveStages
                                .Where(m => m.BidReferanceId == wonnedBidRef.Id &&
                                m.ComparisonChartId == wonnedComp.ComparisonChart.Id &&
                                m.Stage == comparisonChartLastStage).Select(m => m.BidReferanceItemRowPointer).ToListAsync();
                            var discountedTotalPrice = wonnedBidRef.RequestItems.Where(m => wonnedLines.Contains(m.RowPointer) && m.BIDReferanceId == wonnedBidRef.Id).Select(m => m.TotalPrice - m.Discount).Sum();
                            var discount = wonnedBidRef.DiscountPrice > 0 ? wonnedBidRef.DiscountPrice : wonnedBidRef.RequestItems.Where(m => wonnedLines.Contains(m.RowPointer) && m.BIDReferanceId == wonnedBidRef.Id).Select(m => m.Discount).Sum();
                            var currency = wonnedBidRef.Currency;
                            if (currency == "USD")
                            {
                                SetDiscountTotalValues(result, wonnedComp.ComparisonChartPrepared, discountedTotalPrice, discount);
                            }
                            else
                            {
                                var convertedTotalPrice = discountedTotalPrice == 0 ? new ValConvertorDto() : new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(currency, discountedTotalPrice, wonnedBidRef.ComparisonDate.ToString("dd-MM-yyyy"), site.SiteName);
                                var convertedDiscount = discount == 0 ? new ValConvertorDto() : new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(currency, discount, wonnedBidRef.ComparisonDate.ToString("dd-MM-yyyy"), site.SiteName);
                                SetDiscountTotalValues(result, wonnedComp.ComparisonChartPrepared, Convert.ToDecimal(convertedTotalPrice.USD), Convert.ToDecimal(convertedDiscount.USD));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
            #region LocalFunctions
            async Task<List<BIDComparison>> GetWonnedComparisons(SiteDto site, List<string> userGroupBuyers, List<GroupSiteWarehouse> userGroupSiteWarehouses, SBDbContext context)
            {
                var wonnedBidCompariosons = new List<BIDComparison>();

                var userAcceptedComaprisonsByBuyer = await context.BIDComparisons
                        .Include(m => m.ComparisonChart).ThenInclude(m => m.ApproveStage).ThenInclude(m => m.ApproveStageDetails)
                        .Include(m => m.BIDReferances.Where(m => m.WonStatusId == (byte)WonStatuses.Won))
                        .Include(m => m.BIDReferances).ThenInclude(m => m.RequestItems)
                        .Where(m => userGroupBuyers.Contains(
                            m.ComparisonChartPrepared) &&
                            m.ComparisonChart.ApproveStatusId == (byte)ApproveStatuses.Approved &&
                            m.ComparisonChart.CreatedDate >= dateFrom && m.ComparisonChart.CreatedDate <= dateTo)
                        .ToListAsync();

                foreach (var bidComparison in userAcceptedComaprisonsByBuyer)
                {
                    var _comparisonReferance = bidComparison.BIDReferances.FirstOrDefault();
                    if (userGroupSiteWarehouses.Any(m => m.WarehouseCode.Trim() == _comparisonReferance.ProjectWarehouse.Split('-')[0].Trim() && m.SiteId == site.Id))
                    {
                        wonnedBidCompariosons.Add(bidComparison);
                    }
                }
                return wonnedBidCompariosons;
            }

            static void SetDiscountTotalValues(List<NameDiscountTotal> result, string buyer, decimal totalPrice, decimal discount)
            {
                var existData = result.Where(m => m.Name == buyer).FirstOrDefault();
                if (existData == null)
                {
                    result.Add(new NameDiscountTotal
                    {
                        Discount = discount,
                        Name = buyer,
                        Total = totalPrice
                    });
                }
                else
                {
                    existData.Discount = existData.Discount + discount;
                    existData.Total = existData.Total + totalPrice;

                }
            }
            #endregion
        }

        public async Task<List<CostSavedReportModel>> GetCostReport(DateTime dateFrom, DateTime dateTo, string userId, string siteId)
        {
            var result = new List<CostSavedReportModel>();
            var site = await new SiteLogic().GetSite(siteId);
            var buyersBidStatistics = await GetDiscountReport(dateFrom, dateTo, userId, siteId);
            var userGroupBuyers = await new ComparisonChartLogic().GetUserGroupBuyersByUserId(userId, site.SiteDatabase);

            result.AddRange(from costItem in buyersBidStatistics
                            select new CostSavedReportModel
                            {
                                TotalBidAmount = costItem.Total + costItem.Discount,
                                CostSaved = costItem.Discount,
                                BuyerName = costItem.Name
                            });
            result.AddRange(from buyer in userGroupBuyers
                            where !result.Any(m => m.BuyerName == buyer)
                            select new CostSavedReportModel
                            {
                                BuyerName = buyer,
                                CostSaved = 0,
                                TotalBidAmount = 0
                            });
            return result;
        }

    }
}
