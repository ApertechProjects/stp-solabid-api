using Microsoft.EntityFrameworkCore;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class DiscountTypeLogic
    {
        public async Task<List<KeyValueTextBoxingDto>> GetDiscountTypesAsKVP()
        {
            var result = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var datas = await context.DiscountTypes.ToListAsync();
                foreach (var discount in datas)
                {
                    result.Add(new KeyValueTextBoxingDto
                    {
                        Key = discount.Id.ToString(),
                        Value = discount.Id.ToString(),
                        Text = discount.DiscountTypeName
                    });
                }
            }
            return result;
        }
    }
}
