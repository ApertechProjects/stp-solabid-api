using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Dtos.EntityDtos.CarNumber;
using SolaBid.Business.Logics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class CarNumberController : ControllerBase
    {
        public IWebHostEnvironment _env { get; }
        public CarNumberController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult MainAllAndWFA()
        {
            return Ok(new CarNumberLogic().MainAllAndWFA(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        }

        [HttpGet]
        public IActionResult Vendors()
        {
            
            return Ok(new CarNumberLogic().Vendors());
        }

        [HttpGet("{vendorCode}")]
        public IActionResult OrderList(string vendorCode)
        {
            return Ok(new CarNumberLogic().OrderList(vendorCode));
        }

        [HttpGet("{mainId}")]
        public IActionResult Detail(int mainId)
        {
            return Ok(new CarNumberLogic().Detail(mainId));
        }

        [HttpGet("{mainId}")]
        public IActionResult Approvals(int mainId)
        {
            return Ok(new CarNumberLogic().Approvals(mainId));
        }

        [HttpGet("{mainId}")]
        public IActionResult Attachments(int mainId)
        {
            return Ok(new CarNumberLogic().Attachments(mainId));
        }

        [HttpGet]
        public async Task<IActionResult> AdditionalPrivilege()
        {
            return Ok(await new AdditionalPrivilegeLogic().CarNumberAdditionalPrivilege(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        }

        [HttpPost]
        public IActionResult Save(MainItemsSaveDto mainInfos)
        {
            return Ok(new CarNumberLogic().Save(mainInfos, User.FindFirst(ClaimTypes.NameIdentifier)?.Value,_env.WebRootPath));
        }
    }
}
