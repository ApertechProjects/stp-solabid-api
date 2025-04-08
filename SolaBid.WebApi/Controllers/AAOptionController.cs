using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolaBid.Extensions;
using static SolaBid.Business.Logics.CommonLogic.GetData;

namespace SolaBid.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AAOptionController : ControllerBase
    {
        private IWebHostEnvironment _env;
        public AAOptionController(IWebHostEnvironment env)
        {
            _env = env;
        }
        #region ClassModellingActions
        [HttpPut]
        public void SetClassStructureFN()
        {
            string result = FromQuery($"SELECT * FROM dbo.FN_PERFORMANCE_INDEX_EMPLOYEE_MAIL (3,'2022-01-23')", false).GetDataTableColumNames(_env.ContentRootPath);
        }

        [HttpDelete]
        public void SetClassStructureSP()
        {
            string result = FromQuery($"EXEC dbo.APT_OR_Details 'OR00000008','SOCARSTP'", isSiteLineDb: false).GetDataTableColumNames(_env.ContentRootPath);
        }
        #endregion
    }
}
