using SolaBid.Business.Models;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SolaBid.Business.Logics.CommonLogic
{
    public class GenericMapLogic<T>
    {
        public ApiResult BuildModel(DataTable dataTable, bool isList = false)
        {
            var result = new ApiResult();
            if (dataTable is null)
            {
                result.ErrorList.Add("Data is null.Please,Contact Administration");
                return result;
            }

            result.Data = isList ? dataTable.ConvertToClassListModel<T>() : dataTable.ConvertToClassModel<T>();
            result.OperationIsSuccess = true;
            return result;
        }
    }
}
