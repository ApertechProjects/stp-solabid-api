using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolaBid.Business.ActionFilters
{
    public class ValidateActionFilterWithApiResultModel : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.ActionArguments.SingleOrDefault();
            if (param.Value == null)
            {
                var apiResult = new ApiResult
                {
                    ErrorList = new List<string> { "Object is Null" }
                };
                context.Result = new JsonResult(apiResult);
            }
            else if (!context.ModelState.IsValid)
            {
                var apiResult = new ApiResult();
                foreach (var modelState in context.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        apiResult.ErrorList.Add(error.ErrorMessage);
                    }
                }
                context.Result = new JsonResult(apiResult);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

    }
}
