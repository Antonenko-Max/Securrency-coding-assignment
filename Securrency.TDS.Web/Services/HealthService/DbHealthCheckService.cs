using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Securrency.TDS.Web.DataLayer;

namespace Securrency.TDS.Web.Services.HealthService
{
    public class DbHealthCheckService : IHealthCheck
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly string[] _migrations;

        public DbHealthCheckService(IDbContextFactory dbContextFactory)
        {
            var assembly = Assembly.GetAssembly(typeof(DbHealthCheckService));
            Type migrationType = typeof(Migration);
            _migrations = assembly?
                .GetTypes()
                .Where(t => t != migrationType && migrationType.IsAssignableFrom(t))
                .Select(t =>
                    t.CustomAttributes.First(a => a.AttributeType == typeof(MigrationAttribute)).ConstructorArguments
                        .First().Value?.ToString())
                .OrderBy(t => t)
                .ToArray();

            _dbContextFactory = dbContextFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct)
        {
            using TransactionScope tran = Tran.BeginScope(IsolationLevel.ReadUncommitted);
            using SqlConnection connection = _dbContextFactory.CreateConnection();
            await connection.OpenAsync(ct);

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText =
                "select top(1) MigrationId from Tds.__EFMigrationsHistory order by MigrationId desc";
            object lastMigration = await cmd.ExecuteScalarAsync(ct);
            cmd.CommandText = "select count(*) from Tds.__EFMigrationsHistory";
            object count = await cmd.ExecuteScalarAsync(ct);

            return HealthCheckResult.Healthy(
                CompareMigrationHistoryToAssembly(lastMigration.ToString(), int.Parse(count.ToString()!)));
        }

        private string CompareMigrationHistoryToAssembly(string lastMigration, int migrationCount)
        {
            if (!string.Equals(lastMigration, _migrations!.Last(), StringComparison.InvariantCultureIgnoreCase)
                || migrationCount != _migrations.Length)
            {
                return "DB is not up to date, please, check migrations";
            }

            return "DB is up to date";
        }
    }
}
