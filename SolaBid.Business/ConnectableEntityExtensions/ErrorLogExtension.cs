using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SolaBid.Business.ConnectableEntityExtensions
{
    public static class ErrorLogExtension
    {
        public static async Task ErrorLog(this Exception exception,
          [CallerLineNumber] int callerLineNumber = 0,
          [CallerFilePath] string callerFileName = "",
          [CallerMemberName] string callerMethodName = "")
        {
            using (var context = TransactionConfig.AppDbContext)
            {
                await context.ErrorLogs.AddAsync(new ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorDetail = $"{callerFileName} / {callerMethodName} / {callerLineNumber}",
                    ErrorMessage ="Error Message : " + exception.Message + "Error Inner Message : " + exception.InnerException?.Message,
                    ErrorStackTrace = exception.StackTrace
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
