using Microsoft.EntityFrameworkCore;
using SolaBid.Domain.Models.AppDbContext;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.Logics
{
    public class SequenceLogic
    {
        public async Task UpdateSequence(SBDbContext context,string key)
        {
            var entitySequence = await context.Sequences.Where(m => m.Key == key).FirstOrDefaultAsync();
            if (entitySequence == null)
            {
                await context.Sequences.AddAsync(new Sequence { Key = key, SequenceNumber = 1 });
            }
            else
            {
                entitySequence.SequenceNumber = entitySequence.SequenceNumber + 1;
                context.Sequences.Update(entitySequence);
            }
        }
    }
}
