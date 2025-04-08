using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Business.ActionFilters;
using SolaBid.Business.Dtos.ApiDtos.Register;
using SolaBid.Business.Dtos.SingleObjs;
using SolaBid.Business.Logics;
using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class RegisterController : ControllerBase
    {
        public IWebHostEnvironment _env { get; }

        public RegisterController(IWebHostEnvironment env)
        {
            _env = env;
        }


        [HttpGet]
        public async Task<List<RegisterAttachmentModel>> GetPurchaseAttachments([FromQuery] PurchaseAttachmentGetModelDto purchaseAttachmentGetModelDto)
        => await new RegisterLogic().GetPurchaseAttachments(purchaseAttachmentGetModelDto);

        [HttpGet("{DailyRegisterId}")]
        public async Task<List<RegisterAttachmentModel>> GetSourceAttachments(int DailyRegisterId)
        => await new RegisterLogic().GetSourceAttachments(DailyRegisterId);

        [HttpGet("{dateFrom}/{dateTo}")]
        [RequestSizeLimit(100_000_000)]
        public async Task<RegisterViewDto> GetRegister(DateTime dateFrom, DateTime dateTo)
        => await new RegisterLogic().GetRegister(User.FindFirst(ClaimTypes.System)?.Value, dateFrom, dateTo);

        [HttpGet]
        public async Task<List<PurchaseRegisterDto>> GetPurchasingRegisters()
        => await new RegisterLogic().GetPurchasingRegisters();

        [HttpGet]
        public async Task<SourcingRegisterViewDto> GetSourcingRegisters()
        => await new RegisterLogic().GetSourcingRegisters(User.FindFirst(ClaimTypes.System)?.Value);

        [HttpGet]
        public async Task<string> GetRegisterFile([FromQuery] int DailyRegisterAttachmentId, [FromQuery] string FileName)
        => await new RegisterLogic().GetRegisterFile(DailyRegisterAttachmentId, FileName);

        [HttpGet]
        public async Task<List<AllRegisterDto>> GetAllRegisters()
        => await new RegisterLogic().GetAllRegisters(User.FindFirst(ClaimTypes.System)?.Value);

        [HttpGet]
        public async Task<List<RequestComparisonViewDto>> GetRequestComparisons()
        => await new RegisterLogic().GetRequestComparisons();

        [HttpGet("{tabIndex}")]
        public async Task<string> Export(int tabIndex)
        => await new RegisterLogic().Export(tabIndex, User.FindFirst(ClaimTypes.System)?.Value);

        [HttpPost]
        public async Task<ApiResult> TopRegisterCreate(List<PurchaseRegisterCreateDto> registerCreateDtos)
        => await new RegisterLogic().CreateTopRegister(registerCreateDtos);

        [HttpPost]
        public async Task<ApiResult> BottomRegisterCreate(List<SourceRegisterCreateDto> registerCreateDtos)
        => await new RegisterLogic().CreateBottomRegister(registerCreateDtos);

        [HttpPost]
        [ServiceFilter(typeof(ValidateActionFilterWithApiResultModel))]
        public async Task<ApiResult> CreateRegisterRequestComparison(RequestComparisonCreateDto requestComparisonCreateDto)
        => await new RegisterLogic().CreateRegisterComparison(requestComparisonCreateDto);

        [HttpPost]
        public async Task<ApiResult> Delete(IntegerSingleId id)
        => await new RegisterLogic().Delete(id);

        [HttpPost]
        public async Task<ApiResult> SaveAttachments(AttachmentCreateAndDeleteModel attachmentCreateAndDeleteModel)
        => await new RegisterLogic().SaveAttachments(attachmentCreateAndDeleteModel, _env.WebRootPath);

        [HttpPost()]
        public ApiResult ChangeFolderName(ChangeFolderNameModel model)
       => new RegisterLogic().ChangeFolderName(model);

    }
}
