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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class VendorController : ControllerBase
    {
        public IWebHostEnvironment _env { get; }

        public VendorController(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }
        [HttpGet]
        public async Task<List<VendorDto>> GetVendors()
        {
            var result = await new VendorLogic().GetVendorsForVendorMain(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)
                , User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return result;
        }

        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpPost]
        public async Task<ApiResult> Create(VendorDto vendorDto)
        {
            var result = await new VendorLogic().Create(
                vendorDto,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, _env.WebRootPath);
            return result;
        }


        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        [HttpGet("{vendorId}")]
        public async Task<VendorAttachmentModelDto> GetVendorAttachmentsById(int vendorId)
        {
            string host = "https://" + Request.Host.ToString();
            var result = await new VendorLogic().GetVendorAttachmentsById(vendorId, host);
            return result;
        }



        [HttpGet("{vendorId}")]
        public async Task<VendorEditDto> Edit(int vendorId)
        {
            string host = "https://" + Request.Host.ToString();
            var result = await new VendorLogic().GetVendorEditDatas(
                await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value),
                vendorId,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, host);
            return result;
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 2097152000)]
        [RequestSizeLimit(2097152000)]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> Edit(VendorDto editVendor)
        {
            var result = await new VendorLogic().Edit(editVendor, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, _env.WebRootPath);
            return result;
        }


        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> SaveVendorAttchments(VendorAttachmentModelDto sendedAttachments)
        {
            var result = await new VendorLogic().SaveVendorAttachments(sendedAttachments, _env.WebRootPath);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]

        public async Task<ApiResult> Delete(IntegerSingleId vendorId)
        {
            var result = await new VendorLogic().Delete(vendorId.Id);
            return result;
        }

        [HttpGet]
        public async Task<VendorCreateSelectListItemsDto> GetVendorItemList()
        {
            var result = new VendorLogic().GetVendorItems(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value));
            return result;
        }
        [HttpGet("{currency}")]
        public async Task<List<KeyValueTextBoxingDto>> GetBankCodes(string currency)
        {
            var result = new SiteLineDbLogic(await new SiteLogic().GetSiteDatabase(User.FindFirst(ClaimTypes.System)?.Value)).GetBankCode(currency);
            return result;
        }


        //Insert Datas From SiteLine
        [HttpGet]
        public void InsertVendorsFromSiteLine()
        {
            _ = new VendorLogic().InsertVendorsFromSiteLine("SOCARSL_APP", User.FindFirst(ClaimTypes.NameIdentifier)?.Value /*"409909b5-6594-4daa-bb0e-8bf214c03f0c" Anar m ID*/);
        }
    }
}
