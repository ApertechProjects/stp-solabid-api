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
    public class StatusLogic
    {
        public async Task<List<KeyValueTextBoxingDto>> GetStatusesAsKeyValue()
        {
            var statusListDto = new List<KeyValueTextBoxingDto>();
            using (var context = TransactionConfig.AppDbContext)
            {
                var statusEntities = await context.Statuses.ToListAsync();
                statusListDto = TransactionConfig.Mapper.Map<List<KeyValueTextBoxingDto>>(statusEntities);
            }
            return statusListDto;
        }
    }
}
