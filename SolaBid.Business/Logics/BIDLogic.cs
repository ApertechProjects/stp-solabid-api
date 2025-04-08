using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ConnectableEntityExtensions;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Business.Models.Enum;
using SolaBid.Domain.Models.AppDbContext;
using SolaBid.Domain.Models.Entities;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class BIDLogic
    {
        public async Task<List<RequestNumComparisonMixDto>> GetRequestComparisonList(string siteId, string userId)
        {
            var site = await new SiteLogic().GetSite(siteId);
            var result = new List<RequestNumComparisonMixDto>();

            var siteLineRequestNumbers = new SiteLineDbLogic(site.SiteDatabase).GetRequestNumbers(site.SiteName, userId);
            var entityRequestDtos = new List<BIDRequestDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var entityRequests = await context.BIDRequests.
                    Include(m => m.BIDComparisons).ThenInclude(m => m.ComparisonChart)
                    .ToListAsync();
                entityRequestDtos = TransactionConfig.Mapper.Map<List<BIDRequestDto>>(entityRequests);
            }
            foreach (var (reqNum, index) in siteLineRequestNumbers.Select((v, i) => (v, i)))
            {
                var addedData = new RequestNumComparisonMixDto();
                if (entityRequestDtos.Any(m => m.RequestNumber == reqNum))
                {
                    var entityRequestWithComparison = entityRequestDtos.Where(m => m.RequestNumber == reqNum).First();
                    if (entityRequestWithComparison.BIDComparisons.Count > 0)
                    {
                        foreach (var comparison in entityRequestWithComparison.BIDComparisons)
                        {
                            addedData = new RequestNumComparisonMixDto();
                            addedData.ComparisonId = comparison.Id;
                            addedData.ComparisonName = comparison.ComparisonNumber;
                            addedData.RequestNumber = reqNum;
                            addedData.Id = index;
                            addedData.canGenerateBid = comparison.ComparisonChart == null ? true : false;
                            result.Add(addedData);
                        }
                    }
                    else
                    {
                        using (var context = TransactionConfig.AppDbContext)
                        {
                            var emptyRequests = await context.BIDRequests.Where(m => m.RequestNumber == reqNum).FirstOrDefaultAsync();
                            if (emptyRequests != null)
                                context.BIDRequests.Remove(emptyRequests);
                            await context.SaveChangesAsync();
                        }

                        addedData.RequestNumber = reqNum;
                        addedData.Id = index;
                        result.Add(addedData);
                    }
                }
                else
                {
                    addedData.RequestNumber = reqNum;
                    addedData.Id = index;
                    result.Add(addedData);
                }
            }
            return result;
        }
        public async Task<List<KeyValueTextBoxingDto>> GetStatuses()
        {
            var result = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var entityStatuses = await context.Statuses.ToListAsync();
                result = entityStatuses.Select(m => new KeyValueTextBoxingDto
                {
                    Key = m.Id.ToString(),
                    Text = m.StatusName,
                    Value = m.Id.ToString()
                }).ToList();
            }
            return result;
        }

        public async Task<ApiResult> RecalculateBid(RecalculateBidDto recalculateBid)
        {
            var result = new ApiResult();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var bidEntity = await context.BIDReferances.FindAsync(recalculateBid.BidReferanceId);
                    if (bidEntity is null)
                    {
                        result.ErrorList.Add("Bid Not Found!");
                        return result;
                    }
                    bidEntity.AZNTotal = recalculateBid.TotalAZN;
                    bidEntity.USDTotal = recalculateBid.TotalUSD;
                    context.BIDReferances.Update(bidEntity);
                    await context.SaveChangesAsync();
                    _ = RecalculateBidItemUniqueCurrencyUnitPrice(bidEntity.Id);
                    result.OperationIsSuccess = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                result.ErrorList.Add("Operation Failed!");
            }

            return result;
        }

        public async Task RecalculateBidORs()
        {
            using (var context = TransactionConfig.AppDbContext)
            {

                var biDReferances = await context.BIDReferances.Include(m=>m.Site).ToListAsync();
                foreach (var bidReferance in biDReferances)
                {
                    bidReferance.OR = new SiteLineDbLogic(bidReferance.Site.SiteDatabase).GetOR(bidReferance.Site.SiteName, bidReferance.Destination);
                }
                context.BIDReferances.UpdateRange(biDReferances);
                context.SaveChanges();
            }
        }

        public async Task<ApiResult> RecalculateBidItemUniqueCurrencyUnitPrice(int bidReferanceId = 0)
        {

            var result = new ApiResult();
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {

                    var biDReferances = await context.BIDReferances
                                  .Include(m => m.Site)
                                  .Include(m => m.RequestItems)
                                  .Where(m => bidReferanceId == 0 ? true : m.Id == bidReferanceId)
                                  .ToListAsync();
                    foreach (var bidReferance in biDReferances)
                    {
                        foreach (var requestItem in bidReferance.RequestItems)
                        {
                            requestItem.UniqueCurrencyUnitPrice = new SiteLineDbLogic(bidReferance.Site.SiteDatabase).GetCurrencyConvertingAZN(bidReferance.Currency, requestItem.UnitPrice, bidReferance.ComparisonDate.ToString("dd.MM.yyyy"))._USD;
                        }
                    }
                    context.BIDReferances.UpdateRange(biDReferances);
                    context.SaveChanges();
                    result.OperationIsSuccess = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await ex.ErrorLog();
                result.ErrorList.Add("Operation Failed!");
            }

            return result;
        }

        public async Task<List<KeyValueTextBoxingDto>> GetApproveStatuses()
        {
            var result = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var entityStatuses = await context.ApproveStatuses.ToListAsync();
                result = entityStatuses.Select(m => new KeyValueTextBoxingDto
                {
                    Key = m.Id.ToString(),
                    Text = m.StatusName,
                    Value = m.Id.ToString()
                }).ToList();
            }
            return result;
        }
        public async Task<List<KeyValueTextBoxingDto>> GetWons()
        {
            var result = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var entityStatuses = await context.WonStatuses.ToListAsync();
                result = entityStatuses.Select(m => new KeyValueTextBoxingDto
                {
                    Key = m.Id.ToString(),
                    Text = m.StatusName,
                    Value = m.Id.ToString()
                }).ToList();
            }
            return result;
        }
        public async Task<BIDReferanceDto> GetBIDDatasByRequestNumber(string requestNumber, string siteName, string siteDatabase, string userId, bool calculateItemCount, int statusId = 0, int comparisonId = 0)
        {
            var requestDateSiteLine = Convert.ToDateTime(new SiteLineDbLogic(siteDatabase).GetRequestDate(siteName, requestNumber));
            var result = new BIDReferanceDto()
            {
                RequestNumber = requestNumber,
                Requester = new SiteLineDbLogic(siteDatabase).GetRequester(siteName, requestNumber),
                Site = new SiteDto { SiteName = siteName },
                EntryDateFormatted = DateTime.Now.ToString("dd.MM.yyyy"),
                RequestDate = requestDateSiteLine,
                ComparisonDateFormatted = DateTime.Now.ToString("yyyy-MM-dd"),
                RequestDateFormatted = requestDateSiteLine.ToString("dd.MM.yyyy"),
                Destination = new SiteLineDbLogic(siteDatabase).GetDestination(siteName, requestNumber),
                ComparisonChartPrepared = await new UserLogic().GetBuyerNameByUserId(userId),
                ProjectWarehouse = new SiteLineDbLogic(siteDatabase).GetProjectWarehouse(siteName, requestNumber),
                Vendors = await new VendorLogic().GetVendors(),
                VendorKeyValues = await new VendorLogic().GetVendorsKeyValue(),
                DiscountTypes = await new DiscountTypeLogic().GetDiscountTypesAsKVP(),
                DeliveryTerms = new SiteLineDbLogic(siteDatabase).GetDeliveryTerms(),
                PaymentTerms = new SiteLineDbLogic(siteDatabase).GetPaymentTerms(),
                Statuses = await GetStatuses(),
                Wons = await GetWons(),
                Currencies = new SiteLineDbLogic(siteDatabase).GetCurrency(),
                BIDRequestItems = new SiteLineDbLogic(siteDatabase).GetRequestLines(siteName, requestNumber, statusId == (int)Statuses.Closed ? 1 : 0),
                ApprovalStatuses = await GetApproveStatuses(),
            };
            if (calculateItemCount)
            {
                var tempReqList = new List<BIDRequestItemDto>();
                using (var context = TransactionConfig.AppDbContext)
                {
                    var createdComparison = await context.BIDComparisons
                        .Include(m => m.BIDRequest)
                        .Include(m => m.BIDReferances)
                        .Include(m => m.BIDReferances).ThenInclude(m => m.RequestItems)
                        .Where(m => m.BIDRequest.RequestNumber == result.RequestNumber && comparisonId == 0 ? true : m.Id < comparisonId)
                        .ToListAsync();
                    //If createdComparison == null then this request initialize .
                    if (createdComparison == null || createdComparison.Count == 0)
                        return result;

                    foreach (var item in result.BIDRequestItems)
                    {
                        decimal currentItemMaxOrderCount = 0;
                        foreach (var perCompar in createdComparison)
                        {
                            var comparisonComplated = perCompar.BIDReferances.Any(m => m.WonStatusId == (byte)WonStatuses.Won);
                            var data = comparisonComplated
                                ? perCompar.BIDReferances.Where(m => m.WonStatusId == (byte)WonStatuses.Won).SelectMany(m => m.RequestItems).Where(m => m.RowPointer == item.RowPointer).ToList()
                                : perCompar.BIDReferances.SelectMany(m => m.RequestItems).Where(m => m.RowPointer == item.RowPointer).ToList();
                            if (data.Count > 0)
                            {
                                var maxData = data.Max(m => m.ConvQuantity);
                                currentItemMaxOrderCount += maxData;
                            }

                        }
                        if (item.Quantity > currentItemMaxOrderCount)
                        {
                            item.Quantity = item.Quantity - currentItemMaxOrderCount;
                            tempReqList.Add(item);
                        }
                    }

                }
                result.BIDRequestItems = tempReqList;
            }
            return result;
        }
        private string MakeIntoSequence(int lastBidReferanceSequence)
        {
            var bidNumberPath = $"BD-{DateTime.Now.ToString("yy")}{DateTime.Now.ToString("MM")}-";
            if (lastBidReferanceSequence == 0)
                return bidNumberPath + "00001";
            string output = (lastBidReferanceSequence + 1).ToString();
            while (output.Length < 5)
                output = "0" + output;
            return bidNumberPath + output;
        }
        public async Task<string> GenereteBidNumber()
        {
            var bidNumberPath = $"BD-{DateTime.Now.ToString("yy")}{DateTime.Now.ToString("MM")}-";
            string generetedCode = string.Empty;
            using (var context = TransactionConfig.AppDbContext)
            {
                int lastBidReferanceSequence = await context.Sequences.Where(m => m.Key == "BidReferance").Select(m => m.SequenceNumber).FirstOrDefaultAsync();
                generetedCode = MakeIntoSequence(lastBidReferanceSequence);
            }
            return generetedCode;
        }
        public async Task<BIDOperationResult> EditBid(BIDReferanceDto editedBid, ModelStateDictionary ModelState, string siteId, string root, string userId)
        {
            var apiResult = new BIDOperationResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var hasBuyer = await context.Users.Where(u => u.Id == userId).Select(m => m.BuyerId).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(hasBuyer))
                {
                    apiResult.OperationIsSuccess = true;
                    apiResult.BidNumber = "00000000";
                    apiResult.ComparisonId = 0;
                    return apiResult;
                }
                var entityBidReferance = await context.BIDReferances
                    .Include(m => m.RequestItems)
                    .Include(m => m.Site)
                    .Include(m => m.Atachments)
                    .Where(m => m.Id == editedBid.Id)
                    .FirstOrDefaultAsync();
                if (entityBidReferance == null)
                {
                    apiResult.ErrorList.Add("BID Reference not found");
                    return apiResult;
                }
                try
                {
                    entityBidReferance.ComparisonDate = editedBid.ComparisonDate;
                    entityBidReferance.ComparisonDeadline = editedBid.ComparisonDeadline;
                    entityBidReferance.Currency = editedBid.Currency;
                    entityBidReferance.DeliveryTerm = editedBid.DeliveryTerm;
                    entityBidReferance.DeliveryDescription = editedBid.DeliveryDescription;
                    entityBidReferance.DeliveryTime = editedBid.DeliveryTime;
                    entityBidReferance.PONumber = null;
                    entityBidReferance.PayementTerm = editedBid.PayementTerm;
                    entityBidReferance.PaymentDescription = editedBid.PaymentDescription;
                    entityBidReferance.VendorId = editedBid.VendorId;
                    entityBidReferance.ExpectedDelivery = editedBid.ExpectedDelivery;
                    entityBidReferance.BudgetBalance = editedBid.BudgetBalance;
                    entityBidReferance.DiscountTypeId = editedBid.DiscountTypeId;
                    entityBidReferance.DiscountedTotalPrice = editedBid.DiscountedTotalPrice;
                    entityBidReferance.DiscountPrice = editedBid.DiscountPrice;
                    entityBidReferance.DiscountValue = editedBid.DiscountValue;
                    entityBidReferance.TotalAmount = editedBid.TotalAmount;
                    entityBidReferance.TotalQuantity = editedBid.TotalQuantity;
                    entityBidReferance.AZNTotal = editedBid.AZNTotal;
                    entityBidReferance.USDTotal = editedBid.USDTotal;
                    if (editedBid.RequestItems.Count == 0) //Eger Bid Comparisonun Bid Referansi tek budursa o zaman ele comparisonu sil
                    {
                        return await Delete(editedBid.Id, root);
                    }
                    else
                    {
                        foreach (var bidReferanceItem in editedBid.RequestItems)
                        {
                            var entityReferanceItem = entityBidReferance.RequestItems.Where(m => m.Id == bidReferanceItem.Id).FirstOrDefault();
                            if (entityReferanceItem != null)
                            {

                                entityReferanceItem.LineDescription = bidReferanceItem.LineDescription;
                                entityReferanceItem.Quantity = bidReferanceItem.Quantity;
                                entityReferanceItem.LineTotalDiscount = bidReferanceItem.LineTotalDiscount;
                                entityReferanceItem.UnitPrice = bidReferanceItem.UnitPrice;
                                entityReferanceItem.LinePercentValue = bidReferanceItem.LinePercentValue;
                                entityReferanceItem.TotalPrice = bidReferanceItem.TotalPrice;
                                entityReferanceItem.Discount = bidReferanceItem.Discount;
                                entityReferanceItem.PUOMFullText = bidReferanceItem.UOMItems.Where(m => m.Value == bidReferanceItem.PUOMValue).Select(m => m.Text).First();
                                entityReferanceItem.PUOMValue = bidReferanceItem.PUOMValue;
                                entityReferanceItem.Conv = bidReferanceItem.Conv;
                                entityReferanceItem.ConvQuantity = bidReferanceItem.ConvQuantity;
                                entityReferanceItem.ConvUnitPrice = bidReferanceItem.ConvUnitPrice;
                                entityReferanceItem.UniqueCurrencyUnitPrice = new SiteLineDbLogic(entityBidReferance.Site.SiteDatabase).GetCurrencyConvertingAZN(entityBidReferance.Currency, bidReferanceItem.UnitPrice, entityBidReferance.ComparisonDate.ToString("dd.MM.yyyy"))._USD;
                            }
                        }
                    }
                    //Attachment Operations
                    var baseFolder = Path.Combine(root, "appfiles", "BIDDocs", editedBid.Id.ToString());
                    var copyIsSuccessful = FileExtensions.CopyFolderToCopy(baseFolder, "BIDDocsCopy", editedBid.Id.ToString(), root);
                    if (copyIsSuccessful)
                    {
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocs", entityBidReferance.Id.ToString()));
                        context.BIDAttachments.RemoveRange(entityBidReferance.Atachments);
                    }
                    //<<<-->>>//

                    if (editedBid.SendedAttachments != null)
                        foreach (var attachment in editedBid.SendedAttachments)
                        {
                            context.BIDAttachments.Add(new BIDAttachment
                            {
                                BIDReferanceId = editedBid.Id,
                                FilePath = await attachment.FileBase64.SaveBIDAttachment(attachment.FileName, root, editedBid.Id),
                                FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                FileName = attachment.FileName,
                            });
                        }

                    context.BIDReferances.Update(entityBidReferance);
                    await context.SaveChangesAsync();
                    FileExtensions.RemoveFolder(root, Path.Combine("BIDDocsCopy", editedBid.Id.ToString()));
                    apiResult.OperationIsSuccess = true;
                    apiResult.BidNumber = entityBidReferance.BIDNumber;
                    return apiResult;
                }
                catch (Exception ex)
                {
                    var baseFolder = Path.Combine(root, "appfiles", "BIDDocsCopy", editedBid.Id.ToString());
                    var copyIsSuccessful = FileExtensions.CopyFolderToCopy(baseFolder, "BIDDocs", editedBid.Id.ToString(), root);
                    if (copyIsSuccessful)
                    {
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocsCopy", editedBid.Id.ToString()));
                    }
                    apiResult.ErrorList.Add(ex.Message);
                    return apiResult;
                }
            }
        }
        async Task<BIDOperationResult> CreateBid(BIDReferanceDto newBid, ModelStateDictionary ModelState, string siteId, string root, string userId)
        {

            var apiResult = new BIDOperationResult();
            var site = await new SiteLogic().GetSite(siteId);
            var lastComparisonNumber = string.Empty;

            using (var context = TransactionConfig.AppDbContext)
            {
                var hasBuyer = await context.Users.Where(u => u.Id == userId).Select(m => m.BuyerId).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(hasBuyer))
                {
                    apiResult.OperationIsSuccess = true;
                    apiResult.BidNumber = "00000000";
                    apiResult.ComparisonId = 0;
                    return apiResult;
                }
                lastComparisonNumber = await context.BIDComparisons
                    .Include(m => m.BIDRequest)
                    .Where(m => m.BIDRequest.RequestNumber == newBid.RequestNumber)
                    .Select(m => m.ComparisonNumber).FirstOrDefaultAsync();
            }
            //var requestLines = new SiteLineDbLogic(site.SiteDatabase).GetRequestLines(site.SiteName, newBid.RequestNumber);
            //var bidItemRowNumbers = newBid.SelectedRequestItems?.Select(m => m.RowPointer).ToList();
            //var bidBudget = requestLines.Where(m => bidItemRowNumbers.Contains(m.RowPointer)).Sum(m => m.Budget); Secilen itemlara gora budceni getirir
            if (string.IsNullOrEmpty(lastComparisonNumber))
            {
                var newBidRequest = new BIDRequest
                {
                    RequestNumber = newBid.RequestNumber
                };

                var newComparison = new BIDComparison
                {
                    ComparisonNumber = newBid.RequestNumber + "/" + 1,
                    ComparisonChartPrepared = newBid.ComparisonChartPrepared,
                    BIDRequestId = 0, //This value setted 204 line!,
                    ProjectSiteId = int.Parse(siteId),
                    CreateDate = DateTime.Now
                };
                var newBidReference = new BIDReferance
                {
                    BIDNumber = await GenereteBidNumber(),
                    Requester = newBid.Requester,
                    EntryDate = DateTime.Now,
                    RequestDate = Convert.ToDateTime(new SiteLineDbLogic(site.SiteDatabase).GetRequestDate(site.SiteName, newBid.RequestNumber)),
                    ComparisonDate = newBid.ComparisonDate,
                    ComparisonDeadline = newBid.ComparisonDeadline,
                    Destination = newBid.Destination,
                    OR = new SiteLineDbLogic(site.SiteDatabase).GetOR(site.SiteName, newBid.Destination),
                    BudgetBalance = newBid.BudgetBalance,
                    ExpectedDelivery = newBid.ExpectedDelivery,
                    PONumber = null,
                    ComparisonChartPrepared = newBid.ComparisonChartPrepared,
                    ProjectWarehouse = newBid.ProjectWarehouse,
                    DeliveryTerm = newBid.DeliveryTerm,
                    PayementTerm = newBid.PayementTerm,
                    PaymentDescription = newBid.PaymentDescription,
                    DeliveryDescription = newBid.DeliveryDescription,
                    DeliveryTime = newBid.DeliveryTime,
                    Currency = newBid.Currency,
                    TotalAmount = newBid.TotalAmount,
                    TotalQuantity = newBid.TotalQuantity,
                    CurrentCurrTotal = newBid.CurrentCurrTotal,
                    AZNTotal = newBid.AZNTotal,
                    USDTotal = newBid.USDTotal,
                    SiteId = int.Parse(siteId),
                    VendorId = newBid.VendorId,
                    DiscountTypeId = newBid.DiscountTypeId,
                    DiscountedTotalPrice = newBid.DiscountedTotalPrice,
                    DiscountPrice = newBid.DiscountPrice,
                    DiscountValue = newBid.DiscountValue,
                    StatusId = (byte)Statuses.Draft,
                    ApproveStatusId = (byte)ApproveStatuses.NotApproved,
                    WonStatusId = (byte)WonStatuses.InProgress,
                    UserId = userId,
                    BIDComparisonId = 0 //This value setted 204 line!
                };


                try
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        await context.BIDRequests.AddAsync(newBidRequest);
                        await context.SaveChangesAsync();
                        newComparison.BIDRequestId = newBidRequest.Id;
                        await context.BIDComparisons.AddAsync(newComparison);
                        await context.SaveChangesAsync();
                        newBidReference.BIDComparisonId = newComparison.Id;
                        await context.BIDReferances.AddAsync(newBidReference);
                        await context.SaveChangesAsync();
                        if (newBid.SelectedRequestItems != null)
                            foreach (var (reqItem, index) in newBid.SelectedRequestItems.Select((m, i) => (m, i)))
                            {
                                    await context.RELComparisonRequestItems.AddAsync(new RELComparisonRequestItem
                                    {
                                        LineDescription = reqItem.LineDescription,
                                        Quantity = reqItem.Quantity,
                                        LineTotalDiscount = reqItem.LineTotalDiscount,
                                        RowPointer = reqItem.RowPointer,
                                        TotalPrice = reqItem.TotalPrice,
                                        BIDReferanceId = newBidReference.Id,
                                        UnitPrice = reqItem.UnitPrice,
                                        LinePercentValue = reqItem.LinePercentValue,
                                        BidLine = index + 1,
                                        Discount = reqItem.Discount,
                                        PUOMFullText = reqItem.UOMItems.Where(m => m.Value == reqItem.PUOMValue).Select(m => m.Text).First(),
                                        PUOMValue = reqItem.PUOMValue,
                                        Conv = reqItem.Conv,
                                        ConvQuantity = reqItem.ConvQuantity,
                                        ConvUnitPrice = reqItem.ConvUnitPrice,
                                        UniqueCurrencyUnitPrice = new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(newBidReference.Currency, reqItem.UnitPrice, newBidReference.ComparisonDate.ToString("dd.MM.yyyy"))._USD
                                    });
                            }
                        if (newBid.SendedAttachments != null)
                            foreach (var attachment in newBid.SendedAttachments)
                            {
                                context.BIDAttachments.Add(new BIDAttachment
                                {
                                    BIDReferanceId = newBidReference.Id,
                                    FilePath = await attachment.FileBase64.SaveBIDAttachment(attachment.FileName, root, newBidReference.Id),
                                    FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                    FileName = attachment.FileName,
                                });
                            }
                        await new SequenceLogic().UpdateSequence(context, "BidReferance");
                        await context.SaveChangesAsync();
                        apiResult.OperationIsSuccess = true;
                        apiResult.BidNumber = newBidReference.BIDNumber;
                        apiResult.ComparisonId = newComparison.Id;
                    }
                }
                catch (Exception ex)
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        var savedRequest = await context.BIDRequests.FindAsync(newBidRequest.Id);
                        var savedComparison = await context.BIDComparisons.FindAsync(newComparison.Id);
                        var savedBidReference = await context.BIDReferances.FindAsync(newBidReference.Id);
                        var savedBidAttachments = await context.BIDAttachments.Where(m => m.BIDReferanceId == newBidReference.Id).ToListAsync();
                        if (savedRequest != null)
                            context.BIDRequests.Remove(savedRequest);
                        if (savedComparison != null)
                            context.BIDComparisons.Remove(savedComparison);
                        if (savedBidReference != null)
                            context.BIDReferances.Remove(savedBidReference);
                        if (savedBidAttachments != null)
                            context.BIDAttachments.RemoveRange(savedBidAttachments);
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocs", newBidReference.Id.ToString()));
                        await context.SaveChangesAsync();
                    }
                    apiResult.ErrorList.Add(ex.Message);
                }
            }
            else
            {

                var newComparison = new BIDComparison();
                using (var context = TransactionConfig.AppDbContext)
                {
                    var lastComparison = await context.BIDComparisons
                        .Include(m => m.BIDRequest)
                        .Where(m => m.BIDRequest.RequestNumber == newBid.RequestNumber)
                        .OrderByDescending(m => m.Id)
                        .FirstOrDefaultAsync();

                    var newComparisonNumber =
                        int.Parse(lastComparison.ComparisonNumber
                        .Substring(lastComparison.ComparisonNumber.IndexOf('/') + 1,
                        lastComparison.ComparisonNumber.Length - (lastComparison.ComparisonNumber.IndexOf('/') + 1))
                        ) + 1;
                    newComparison.ComparisonNumber = lastComparison.BIDRequest.RequestNumber + "/" + newComparisonNumber;
                    newComparison.BIDRequestId = lastComparison.BIDRequestId;
                    newComparison.ComparisonChartPrepared = newBid.ComparisonChartPrepared;
                    newComparison.CreateDate = DateTime.Now;
                }


                var newBidReference = new BIDReferance
                {
                    BIDNumber = await GenereteBidNumber(),
                    Requester = newBid.Requester,
                    EntryDate = DateTime.Now,
                    RequestDate = Convert.ToDateTime(new SiteLineDbLogic(site.SiteDatabase).GetRequestDate(site.SiteName, newBid.RequestNumber)),
                    ComparisonDate = newBid.ComparisonDate,
                    ComparisonDeadline = newBid.ComparisonDeadline,
                    Destination = newBid.Destination,
                    OR = new SiteLineDbLogic(site.SiteDatabase).GetOR(site.SiteName, newBid.Destination),
                    PONumber = null,
                    BudgetBalance = newBid.BudgetBalance,
                    ExpectedDelivery = newBid.ExpectedDelivery,
                    ComparisonChartPrepared = newBid.ComparisonChartPrepared,
                    ProjectWarehouse = newBid.ProjectWarehouse,
                    DeliveryTerm = newBid.DeliveryTerm,
                    PayementTerm = newBid.PayementTerm,
                    PaymentDescription = newBid.PaymentDescription,
                    DeliveryDescription = newBid.DeliveryDescription,
                    DeliveryTime = newBid.DeliveryTime,
                    Currency = newBid.Currency,
                    TotalAmount = newBid.TotalAmount,
                    TotalQuantity = newBid.TotalQuantity,
                    CurrentCurrTotal = newBid.CurrentCurrTotal,
                    AZNTotal = newBid.AZNTotal,
                    USDTotal = newBid.USDTotal,
                    SiteId = int.Parse(siteId),
                    DiscountTypeId = newBid.DiscountTypeId,
                    DiscountedTotalPrice = newBid.DiscountedTotalPrice,
                    DiscountPrice = newBid.DiscountPrice,
                    DiscountValue = newBid.DiscountValue,
                    VendorId = newBid.VendorId,
                    StatusId = (byte)Statuses.Draft,
                    ApproveStatusId = (byte)ApproveStatuses.NotApproved,
                    WonStatusId = (byte)WonStatuses.InProgress,
                    UserId = userId,
                    BIDComparisonId = 0 //This value setted 204 line!
                };

                try
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {

                        await context.BIDComparisons.AddAsync(newComparison);
                        await context.SaveChangesAsync();
                        newBidReference.BIDComparisonId = newComparison.Id;
                        await context.BIDReferances.AddAsync(newBidReference);
                        await context.SaveChangesAsync();
                        if (newBid.SelectedRequestItems != null)
                            foreach (var (reqItem, index) in newBid.SelectedRequestItems.Select((m, i) => (m, i)))
                            {
                                if (reqItem.IsSelected)
                                {
                                    await context.RELComparisonRequestItems.AddAsync(new RELComparisonRequestItem
                                    {
                                        LineTotalDiscount = reqItem.LineTotalDiscount,
                                        LineDescription = reqItem.LineDescription,
                                        Quantity = reqItem.Quantity,
                                        RowPointer = reqItem.RowPointer,
                                        TotalPrice = reqItem.TotalPrice,
                                        BIDReferanceId = newBidReference.Id,
                                        UnitPrice = reqItem.UnitPrice,
                                        LinePercentValue = reqItem.LinePercentValue,
                                        BidLine = index + 1,
                                        Discount = reqItem.Discount,
                                        PUOMFullText = reqItem.UOMItems.Where(m => m.Value == reqItem.PUOMValue).Select(m => m.Text).First(),
                                        PUOMValue = reqItem.PUOMValue,
                                        Conv = reqItem.Conv,
                                        ConvQuantity = reqItem.ConvQuantity,
                                        ConvUnitPrice = reqItem.ConvUnitPrice,
                                        UniqueCurrencyUnitPrice = new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(newBidReference.Currency, reqItem.UnitPrice, newBidReference.ComparisonDate.ToString("dd.MM.yyyy"))._USD
                                    });
                                }

                            }
                        if (newBid.SendedAttachments != null)
                            foreach (var attachment in newBid.SendedAttachments)
                            {
                                context.BIDAttachments.Add(new BIDAttachment
                                {
                                    BIDReferanceId = newBidReference.Id,
                                    FilePath = await attachment.FileBase64.SaveBIDAttachment(attachment.FileName, root, newBidReference.Id),
                                    FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                    FileName = attachment.FileName,
                                });
                            }
                        await new SequenceLogic().UpdateSequence(context, "BidReferance");
                        await context.SaveChangesAsync();
                        apiResult.BidNumber = newBidReference.BIDNumber;
                        apiResult.ComparisonId = newComparison.Id;
                        apiResult.OperationIsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    using (var context = TransactionConfig.AppDbContext)
                    {
                        var savedComparison = await context.BIDComparisons.FindAsync(newComparison.Id);
                        var savedBidReference = await context.BIDReferances.FindAsync(newBidReference.Id);
                        var savedBidAttachments = await context.BIDAttachments.Where(m => m.BIDReferanceId == newBidReference.Id).ToListAsync();

                        if (savedComparison != null)
                            context.BIDComparisons.Remove(savedComparison);
                        if (savedBidReference != null)
                            context.BIDReferances.Remove(savedBidReference);
                        if (savedBidAttachments != null)
                            context.BIDAttachments.RemoveRange(savedBidAttachments);
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocs", newBidReference.Id.ToString()));
                        await context.SaveChangesAsync();
                    }
                    apiResult.ErrorList.Add(ex.Message);
                }
            }


            return apiResult;
        }
        async Task<BIDOperationResult> CreateBidtoExistComparison(BIDReferanceDto newBid, ModelStateDictionary ModelState, string siteId, string root, string userId)
        {
            var apiResult = new BIDOperationResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var hasBuyer = await context.Users.Where(u => u.Id == userId).Select(m => m.BuyerId).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(hasBuyer))
                {
                    apiResult.OperationIsSuccess = true;
                    apiResult.BidNumber = "00000000";
                    apiResult.ComparisonId = 0;
                    return apiResult;
                }
            }
            var site = await new SiteLogic().GetSite(siteId);
            var newBidReference = new BIDReferance
            {
                BIDNumber = await GenereteBidNumber(),
                Requester = newBid.Requester,
                EntryDate = DateTime.Now,
                RequestDate = Convert.ToDateTime(new SiteLineDbLogic(site.SiteDatabase).GetRequestDate(site.SiteName, newBid.RequestNumber)),
                ComparisonDate = newBid.ComparisonDate,
                ComparisonDeadline = newBid.ComparisonDeadline,
                Destination = newBid.Destination,
                OR = new SiteLineDbLogic(site.SiteDatabase).GetOR(site.SiteName, newBid.Destination),
                PONumber = null,
                ComparisonChartPrepared = newBid.ComparisonChartPrepared,
                ProjectWarehouse = newBid.ProjectWarehouse,
                DeliveryTerm = newBid.DeliveryTerm,
                PayementTerm = newBid.PayementTerm,
                PaymentDescription = newBid.PaymentDescription,
                DeliveryDescription = newBid.DeliveryDescription,
                DeliveryTime = newBid.DeliveryTime,
                Currency = newBid.Currency,
                TotalAmount = newBid.TotalAmount,
                TotalQuantity = newBid.TotalQuantity,
                CurrentCurrTotal = newBid.CurrentCurrTotal,
                AZNTotal = newBid.AZNTotal,
                USDTotal = newBid.USDTotal,
                SiteId = int.Parse(siteId),
                VendorId = newBid.VendorId,
                DiscountTypeId = newBid.DiscountTypeId,
                DiscountedTotalPrice = newBid.DiscountedTotalPrice,
                DiscountPrice = newBid.DiscountPrice,
                DiscountValue = newBid.DiscountValue,
                StatusId = (byte)Statuses.Draft,
                ApproveStatusId = (byte)ApproveStatuses.NotApproved,
                WonStatusId = (byte)WonStatuses.InProgress,
                ExpectedDelivery = newBid.ExpectedDelivery,
                BudgetBalance = newBid.BudgetBalance,
                UserId = userId,
                BIDComparisonId = newBid.BIDComparisonId
            };
            try
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    await context.BIDReferances.AddAsync(newBidReference);
                    await context.SaveChangesAsync();
                    if (newBid.SelectedRequestItems != null)
                        foreach (var (reqItem, index) in newBid.SelectedRequestItems.Select((m, i) => (m, i)))
                        {
                            await context.RELComparisonRequestItems.AddAsync(new RELComparisonRequestItem
                            {
                                LineTotalDiscount = reqItem.LineTotalDiscount,
                                LineDescription = reqItem.LineDescription,
                                Quantity = reqItem.Quantity,
                                RowPointer = reqItem.RowPointer,
                                TotalPrice = reqItem.TotalPrice,
                                BIDReferanceId = newBidReference.Id,
                                LinePercentValue = reqItem.LinePercentValue,
                                UnitPrice = reqItem.UnitPrice,
                                BidLine = index + 1,
                                Discount = reqItem.Discount,
                                PUOMFullText = reqItem.UOMItems.Where(m => m.Value == reqItem.PUOMValue).Select(m => m.Text).First(),
                                PUOMValue = reqItem.PUOMValue,
                                Conv = reqItem.Conv,
                                ConvQuantity = reqItem.ConvQuantity,
                                ConvUnitPrice = reqItem.ConvUnitPrice,
                                UniqueCurrencyUnitPrice = new SiteLineDbLogic(site.SiteDatabase).GetCurrencyConvertingAZN(newBidReference.Currency, reqItem.UnitPrice, newBidReference.ComparisonDate.ToString("dd.MM.yyyy"))._USD
                            });
                        }
                    if (newBid.SendedAttachments != null)
                        foreach (var attachment in newBid.SendedAttachments)
                        {
                            context.BIDAttachments.Add(new BIDAttachment
                            {
                                BIDReferanceId = newBidReference.Id,
                                FilePath = await attachment.FileBase64.SaveBIDAttachment(attachment.FileName, root, newBidReference.Id),
                                FileBaseType = attachment.FileBase64.GetFileBaseType(),
                                FileName = attachment.FileName,
                            });
                        }
                    await new SequenceLogic().UpdateSequence(context, "BidReferance");
                    await context.SaveChangesAsync();
                    apiResult.OperationIsSuccess = true;
                    apiResult.BidNumber = newBidReference.BIDNumber;
                    apiResult.ComparisonId = newBidReference.BIDComparisonId;
                }
            }
            catch (Exception ex)
            {
                using (var context = TransactionConfig.AppDbContext)
                {
                    var savedBidReference = await context.BIDReferances.FindAsync(newBidReference.Id);
                    var savedBidAttachments = await context.BIDAttachments.Where(m => m.BIDReferanceId == newBidReference.Id).ToListAsync();
                    if (savedBidReference != null)
                        context.BIDReferances.Remove(savedBidReference);
                    if (savedBidAttachments != null)
                        context.BIDAttachments.RemoveRange(savedBidAttachments);
                    FileExtensions.RemoveFolder(root, Path.Combine("BIDDocs", newBidReference.Id.ToString()));

                    await context.SaveChangesAsync();
                }
                apiResult.ErrorList.Add(ex.Message);
            }
            return apiResult;
        }
        public async Task<BIDOperationResult> CreateOrEditOrGenerateExistComparison(BIDReferanceDto newBid, ModelStateDictionary ModelState, string siteId, string root, string userId)
        {
            if (newBid.Id == 0 && newBid.BIDComparisonId == 0) //Create New Bid And New Comparison
            {
                return await CreateBid(newBid, ModelState, siteId, root, userId);
            }
            else if (newBid.Id == 0 && newBid.BIDComparisonId != 0) //Create New Bid To Exist Comparison
            {
                return await CreateBidtoExistComparison(newBid, ModelState, siteId, root, userId);
            }
            else if (newBid.Id != 0 && newBid.BIDComparisonId != 0) //Edit Bid Card
            {
                return await EditBid(newBid, ModelState, siteId, root, userId);
            }
            else
            {
                return null;
            }
        }
        public async Task<BIDReferanceDto> GetEditBID(int bidId, string host)
        {
            var result = new BIDReferanceDto();
            using (var context = TransactionConfig.AppDbContext)
            {
                var bidEntity = await context.BIDReferances
                    .Include(m => m.Atachments)
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDRequest)
                    .Include(m => m.RequestItems)
                    .Include(m => m.Site)
                    .Include(m => m.Vendor)
                    .Where(m => m.Id == bidId)
                    .FirstOrDefaultAsync();
                result.ComparisanDataIsEditable = bidEntity.BIDComparison.BIDReferances.Count > 1 ? true : false;
                result = TransactionConfig.Mapper.Map<BIDReferanceDto>(bidEntity);
                result.Attachments = TransactionConfig.Mapper.Map<List<BIDAttachmentDto>>(bidEntity.Atachments);
                if (bidEntity == null)
                    return null;
                var bidRequestNumber = bidEntity.BIDComparison.BIDRequest.RequestNumber;
                var bidItemsModel = await GetBIDDatasByRequestNumber(bidRequestNumber, bidEntity.Site.SiteName, bidEntity.Site.SiteDatabase, bidEntity.UserId, true, bidEntity.StatusId, bidEntity.BIDComparisonId);
                result.RequestNumber = bidRequestNumber;
                result.Statuses = bidItemsModel.Statuses;
                result.ApprovalStatuses = bidItemsModel.ApprovalStatuses;
                result.Wons = bidItemsModel.Wons;
                result.DeliveryTerms = bidItemsModel.DeliveryTerms;
                result.PaymentTerms = bidItemsModel.PaymentTerms;
                result.Currencies = bidItemsModel.Currencies;
                result.VendorKeyValues = bidItemsModel.VendorKeyValues;
                result.DiscountTypes = bidItemsModel.DiscountTypes;
                result.Vendors = bidItemsModel.Vendors;
                result.ComparisonDateFormatted = result.ComparisonDate.ToString("yyyy-MM-dd");
                result.ComparisonDeadlineFormatted = result.ComparisonDeadline.ToString("yyyy-MM-dd");
                result.EntryDateFormatted = result.EntryDate.ToString("dd.MM.yyyy");
                result.RequestDateFormatted = result.RequestDate.ToString("dd.MM.yyyy");
                result.DiscountTypes = bidItemsModel.DiscountTypes;
                var chartIsExist = result.BIDComparison.ComparisonChart == null;

                foreach (var (item, i) in result.RequestItems.Select((v, i) => (v, i)))
                {
                    var slItem = bidItemsModel.BIDRequestItems.Where(m => m.RowPointer == item.RowPointer).FirstOrDefault();
                    if (slItem != null)
                    {
                        result.BidItemsForEdit.Add(new ReqItemCombinationDto
                        {
                            SequenceNumber = i + 1,
                            Id = item.Id,
                            BidLine = slItem.RequestLine,
                            Budget = slItem.Budget,
                            RefType = slItem.RefType,
                            LineDescription = item.LineDescription,
                            Quantity = item.Quantity,
                            RequestQuantity = slItem.Quantity,
                            RowPointer = item.RowPointer,
                            TotalPrice = item.TotalPrice,
                            LinePercentValue = item.LinePercentValue,
                            UnitPrice = item.UnitPrice,
                            UOM = slItem.UOM,
                            ItemName = slItem.ItemName,
                            Discount = item.Discount,
                            LineTotalDiscount = item.LineTotalDiscount,
                            UOMItems = slItem.UOMItems,
                            Conv = chartIsExist ? item.Conv : slItem.UOMItems.Where(m => m.Value == item.PUOMValue).First().Conv,
                            ConvQuantity = chartIsExist ? item.ConvQuantity : item.Quantity / slItem.UOMItems.Where(m => m.Value == item.PUOMValue).First().Conv,
                            ConvUnitPrice = item.UnitPrice * slItem.UOMItems.Where(m => m.Value == item.PUOMValue).First().Conv,
                            PUOMValue = item.PUOMValue
                        });
                    }
                }
                foreach (var attachm in result.Attachments)
                {

                    result.SendedAttachments.Add(new SendedAttachmentDto
                    {
                        FileBase64 = FileExtensions.ConvertFileToBase64(attachm.FilePath, "appfiles", attachm.FileBaseType),
                        FileName = attachm.FileName,
                        FileUrl = Path.Combine(host, "appfiles", attachm.FilePath),
                        FileId = attachm.Id.ToString()
                    });
                }
            }

            result.Vendor = TransactionConfig.Mapper.Map<VendorDto>(result.Vendor);
            result.BIDComparison = TransactionConfig.Mapper.Map<BIDComparisonDto>(result.BIDComparison);
            result.Site = TransactionConfig.Mapper.Map<SiteDto>(result.Site);
            result.RequestItems = TransactionConfig.Mapper.Map<List<RELComparisonRequestItemDto>>(result.RequestItems);
            result.Attachments = TransactionConfig.Mapper.Map<List<BIDAttachmentDto>>(result.Attachments);
            result.BidItemsForEdit = result.BidItemsForEdit.OrderBy(m => m.BidLine).ToList();

            return result;
        }
        public async Task<BIDReferanceDto> GetGenerateNewExistComparisonBIDData(int comparisonId)
        {
            var result = new BIDReferanceDto();
            using (var context = TransactionConfig.AppDbContext)
            {
                var bidEntity = await context.BIDReferances
                    .Include(m => m.RequestItems)
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDRequest)
                    .Include(m => m.Site)
                    .Where(m => m.BIDComparisonId == comparisonId)
                    .FirstOrDefaultAsync();
                result = TransactionConfig.Mapper.Map<BIDReferanceDto>(bidEntity);
                if (bidEntity == null)
                    return null;
                var bidRequestNumber = bidEntity.BIDComparison.BIDRequest.RequestNumber;
                var bidItemsModel = await GetBIDDatasByRequestNumber(bidRequestNumber, bidEntity.Site.SiteName, bidEntity.Site.SiteDatabase, bidEntity.UserId, true, bidEntity.StatusId, comparisonId);
                result.RequestNumber = bidRequestNumber;
                result.VendorId = 0;
                result.Id = 0;
                result.TotalAmount = 0;
                result.ComparisonChartPrepared = result.BIDComparison.ComparisonChartPrepared;
                result.TotalQuantity = 0;
                result.DeliveryTerm = result.DeliveryTime = result.PayementTerm = result.PaymentDescription = result.DeliveryDescription = result.Currency = result.BIDNumber = string.Empty;
                result.AZNTotal = result.USDTotal = result.DiscountPrice = result.DiscountedTotalPrice = result.DiscountValue = result.TotalAmount = result.TotalQuantity = 0;
                result.DiscountTypeId = (byte)DiscountTypes.Empty;
                result.Statuses = bidItemsModel.Statuses;
                result.ApprovalStatuses = bidItemsModel.ApprovalStatuses;
                result.Wons = bidItemsModel.Wons;
                result.DeliveryTerms = bidItemsModel.DeliveryTerms;
                result.PaymentTerms = bidItemsModel.PaymentTerms;
                result.Currencies = bidItemsModel.Currencies;
                result.DiscountTypes = bidItemsModel.DiscountTypes;
                result.VendorKeyValues = bidItemsModel.VendorKeyValues;
                result.EntryDate = DateTime.Now;
                result.EntryDateFormatted = DateTime.Now.ToString("dd.MM.yyyy");
                result.Vendors = bidItemsModel.Vendors;
                result.ComparisonDateFormatted = result.ComparisonDate.ToString("yyyy-MM-dd");
                result.ComparisonDeadlineFormatted = result.ComparisonDeadline.ToString("yyyy-MM-dd");
                result.RequestDate = bidItemsModel.RequestDate;
                foreach (var item in result.RequestItems)
                {
                    var slItem = bidItemsModel.BIDRequestItems.Where(m => m.RowPointer == item.RowPointer).FirstOrDefault();
                    if (slItem != null)
                    {
                        result.BidItemsForEdit.Add(new ReqItemCombinationDto
                        {
                            BidLine = slItem.RequestLine,
                            RefType = slItem.RefType,
                            Budget = slItem.Budget,
                            RequestQuantity = slItem.Quantity,
                            RowPointer = item.RowPointer,
                            UOM = slItem.UOM,
                            ItemName = slItem.ItemName,
                            Discount = 0,
                            LineTotalDiscount = 0,
                            UOMItems = slItem.UOMItems,
                            Conv = 1,
                            PUOMValue = slItem.UOM

                        });
                    }
                }
            }

            result.BIDComparison = TransactionConfig.Mapper.Map<BIDComparisonDto>(result.BIDComparison);
            result.Site = TransactionConfig.Mapper.Map<SiteDto>(result.Site);
            result.RequestItems = TransactionConfig.Mapper.Map<List<RELComparisonRequestItemDto>>(result.RequestItems);
            result.BidItemsForEdit = result.BidItemsForEdit.OrderBy(m => m.BidLine).ToList();

            return result;
        }
        public async Task<List<BIDReferanceDto>> GetBidReferances(string siteId, string userId)
        {

            var result = new List<BIDReferanceDto>();
            var site = await new SiteLogic().GetSite(siteId);
            var userGroupBuyers = await new ComparisonChartLogic().GetUserGroupBuyersByUserId(userId, site.SiteDatabase);
            //var userGroupSiteWarehouses = await new ComparisonChartLogic().GetUserGroupSiteWarehouses(userId, site.Id);
            using (var context = TransactionConfig.AppDbContext)
            {
                var bidReferanceEntities = await context.BIDReferances
                        .Include(m => m.Site)
                        .Include(m => m.Status)
                        .Include(m => m.WonStatus)
                        .Include(m => m.Vendor)
                        .Include(m => m.BIDComparison).ThenInclude(m => m.BIDRequest)
                        .Where(m => m.SiteId == site.Id && userGroupBuyers.Contains(m.ComparisonChartPrepared))
                        .OrderByDescending(m => m.Id)
                        .ToListAsync();
                result = TransactionConfig.Mapper.Map<List<BIDReferanceDto>>(bidReferanceEntities);
                foreach (var bidReferance in result)
                {
                    bidReferance.ComparisonDeadlineFormatted = bidReferance.ComparisonDeadline.ToString("dd/MM/yyyy");
                    bidReferance.Status = TransactionConfig.Mapper.Map<StatusDto>(bidReferance.Status);
                    bidReferance.Site = TransactionConfig.Mapper.Map<SiteDto>(bidReferance.Site);
                    bidReferance.WonStatus = TransactionConfig.Mapper.Map<WonStatusDto>(bidReferance.WonStatus);
                    bidReferance.Vendor = TransactionConfig.Mapper.Map<VendorDto>(bidReferance.Vendor);
                    bidReferance.BIDComparison = TransactionConfig.Mapper.Map<BIDComparisonDto>(bidReferance.BIDComparison);
                    bidReferance.RequestNumber = bidReferance.BIDComparison.BIDRequest.RequestNumber;
                }
            }


            return result;
        }
        public async Task<BIDOperationResult> Delete(int bidReferanceId, string root)
        {
            var apiResult = new BIDOperationResult();

            using (var context = TransactionConfig.AppDbContext)
            {
                var findedBidRequest = await context.BIDReferances
                    .Include(m => m.Status)
                    .Include(m => m.Atachments)
                    .Include(m => m.BIDComparison).ThenInclude(m => m.BIDReferances)
                    .Where(m => m.Id == bidReferanceId).FirstOrDefaultAsync();
                try
                {
                    if (findedBidRequest.StatusId != (byte)Statuses.Draft)
                    {
                        apiResult.ErrorList.Add($"Operation Failed.Because this Request Status is {findedBidRequest.Status.StatusName}");
                        return apiResult;
                    }
                    //Attachment Operations

                    var baseFolder = Path.Combine(root, "appfiles", "BIDDocs", findedBidRequest.Id.ToString());
                    var copyIsSuccessfully = FileExtensions.CopyFolderToCopy(baseFolder, "BIDDocsCopy", findedBidRequest.Id.ToString(), root);
                    if (copyIsSuccessfully)
                    {
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocs", findedBidRequest.Id.ToString()));
                        context.BIDAttachments.RemoveRange(findedBidRequest.Atachments);
                    }

                    //<<<-->>>//
                    if (findedBidRequest.BIDComparison.BIDReferances.Count == 1)
                    {
                        context.BIDComparisons.Remove(findedBidRequest.BIDComparison);
                    }
                    else
                    {
                        context.BIDReferances.Remove(findedBidRequest);
                    }
                    await context.SaveChangesAsync();
                    apiResult.BidNumber = findedBidRequest.BIDNumber;
                    apiResult.OperationIsSuccess = true;
                    FileExtensions.RemoveFolder(root, Path.Combine("BIDDocsCopy", findedBidRequest.Id.ToString()));

                    return apiResult;
                }
                catch (Exception ex)
                {
                    var baseFolder = Path.Combine(root, "appfiles", "BIDDocsCopy", findedBidRequest.Id.ToString());
                    var copyIsSuccessfully = FileExtensions.CopyFolderToCopy(baseFolder, "BIDDocs", findedBidRequest.Id.ToString(), root);
                    if (copyIsSuccessfully)
                    {
                        FileExtensions.RemoveFolder(root, Path.Combine("BIDDocsCopy", findedBidRequest.Id.ToString()));
                        apiResult.ErrorList.Add(ex.Message);
                    }
                    return apiResult;
                }
            }
        }
    }
}


