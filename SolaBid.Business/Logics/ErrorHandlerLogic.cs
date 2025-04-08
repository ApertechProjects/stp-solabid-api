using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Logics
{
    public class ErrorHandlerLogic
    {
        public ApiResult GetModelErrors(ModelStateDictionary ModelState)
        {
            var apiErrorResult = new ApiResult();
            foreach (var modelState in ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    apiErrorResult.ErrorList.Add(error.ErrorMessage);
                }
            }
            return apiErrorResult;
        }
    }
}
