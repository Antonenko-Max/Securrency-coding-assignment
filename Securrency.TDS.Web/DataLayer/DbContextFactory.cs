using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Securrency.TDS.Web.DataLayer
{
    public interface IDbContextFactory
    {
        AppDbContext CreateContext();

        SqlConnection CreateConnection();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<DataLayerOptions> _options;

        public DbContextFactory(ILoggerFactory loggerFactory, IOptions<DataLayerOptions> options)
        {
            _loggerFactory = loggerFactory;
            _options = options;
        }

        public AppDbContext CreateContext()
        {
            return new AppDbContext(_options, _loggerFactory);
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_options.Value.ConnectionString);
        }
    }
}