using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SolaBid.Business.ConnectableEntityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolaBid.WebApi.Models
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            await exception.ErrorLog();
            context.ExceptionHandled = true;
            context.Result = new JsonResult("Any Problem Detected.Please Connect To Administration.Problem Logged!");
        }
    }
}
