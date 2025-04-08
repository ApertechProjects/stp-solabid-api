using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Models
{
    public class ApiResult
    {
        public ApiResult(bool operationIsSuccess = false)
        {
            ErrorList = new List<string>();
            OperationIsSuccess = operationIsSuccess;
        }
        public List<string> ErrorList { get; set; }
        public bool OperationIsSuccess { get; set; }
        public bool IsError { get; set; }
        public object Data { get; set; }
    }
}
