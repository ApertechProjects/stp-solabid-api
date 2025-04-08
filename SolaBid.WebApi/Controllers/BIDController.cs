using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BIDController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public BIDController(IWebHostEnvironment env)
        {
            _env = env;
        }

        //GET METHODS START
        [HttpGet]
        public async Task<List<BIDReferanceDto>> GetBidReferances()
        {
            var result = await new BIDLogic().GetBidReferances(User.FindFirst(ClaimTypes.System)?.Value, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }


        [HttpGet]
        public async Task<List<RequestNumComparisonMixDto>> GetRequestComparisonList()
        {
            var result = await new BIDLogic().GetRequestComparisonList(User.FindFirst(ClaimTypes.System)?.Value, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }

        [HttpGet("{requestNumber}")]
        public async Task<BIDReferanceDto> GetBIDDatasByRequestNumber(string requestNumber)
        {
            var result = await new BIDLogic().GetBIDDatasByRequestNumber(requestNumber,
                                   await new SiteLogic().GetSiteName(User.FindFirst(ClaimTypes.System)?.Value),
                                   await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value),
                                   User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                                   true
                                   );
            return result;
        }

        [HttpGet("{bidId}")]
        public async Task<BIDReferanceDto> GetBidDatasForEdit(int bidId)
        {
            string host = "https://" + Request.Host.ToString();
            var result = await new BIDLogic().GetEditBID(bidId, host);
            return result;
        }

        [HttpGet("{comparisonId}")]
        public async Task<BIDReferanceDto> GetBidDatasForGenerateBid(int comparisonId)
        {
            var result = await new BIDLogic().GetGenerateNewExistComparisonBIDData(comparisonId);
            return result;
        }

        [HttpGet("{currency}/{value}/{date}")]
        public async Task<ValConvertorDto> GetConvertedDatas(string currency, decimal value, string date)
        {
            var result = new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetCurrencyConvertingAZN(currency, value, date,
            await new SiteLogic().GetSiteName(User.FindFirst(ClaimTypes.System)?.Value));
            return result;
        }

        [HttpGet("{currency}/{value}")]
        public async Task<ValConvertorDto> GetConvertedDatas(string currency, decimal value)
        {
            var result = new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetCurrencyConvertingAZN(currency, value, DateTime.Now.ToString(),await new SiteLogic().GetSiteName(User.FindFirst(ClaimTypes.System)?.Value));
            return result;
        }

        [HttpGet]
        public async Task<bool> CheckValutaIsFilled()
        {
            return new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).CheckValutaIsFilled(
                await new SiteLogic().GetSiteName(User.FindFirst(ClaimTypes.System)?.Value)
            );
        }

        //GET METHODS END

        //POST METHODS START
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<BIDOperationResult> Delete(IntegerSingleId bidReferanceId)
        {
            var result = await new BIDLogic().Delete(bidReferanceId.Id, _env.WebRootPath);
            return result;
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> RecalculateBid(RecalculateBidDto recalculateBid)
        {
            return await new BIDLogic().RecalculateBid(recalculateBid);
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> RecalculateBidItemUniqueCurrencyUnitPrice()
        {
            return await new BIDLogic().RecalculateBidItemUniqueCurrencyUnitPrice();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task RecalculateBidORs()
        {
            await new BIDLogic().RecalculateBidORs();
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<BIDOperationResult> BidCreateEditOrGenerateForExistComparison(BIDReferanceDto newBid)
        {
            var siteId = User.FindFirst(ClaimTypes.System)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await new BIDLogic().CreateOrEditOrGenerateExistComparison(newBid, ModelState, siteId, _env.WebRootPath, userId);
        }
        //POST METHODS END
    }
}
