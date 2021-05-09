using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Securrency.TDS.Web.DataLayer
{
    public class AppDbContext : DbContext
    {
        private readonly DataLayerOptions _options;
        private readonly ILoggerFactory _loggerFactory;

        public bool SelectWithUpdLock { get; set; }

        public AppDbContext(IOptions<DataLayerOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            builder.UseSqlServer(_options.ConnectionString, options =>
            {
                options.MigrationsHistoryTable("__EFMigrationsHistory", EntityTypeBuilderExtensions.SchemaName);
            });
            if (_loggerFactory != null) builder.UseLoggerFactory(_loggerFactory);
            builder.AddInterceptors(new QueryCommandInterceptor());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}