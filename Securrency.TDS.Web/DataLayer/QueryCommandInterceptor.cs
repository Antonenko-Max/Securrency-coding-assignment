using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Securrency.TDS.Web.DataLayer
{
    public class QueryCommandInterceptor : DbCommandInterceptor
    {
        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            var context = (AppDbContext)eventData.Context;
            if (context.SelectWithUpdLock)
            {
                command.CommandText = Regex.Replace(command.CommandText, @"(AS \[.])", @"$1 WITH (UPDLOCK)");
            }
            return Task.FromResult(result);
        }
    }
}
