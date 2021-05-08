using Microsoft.Extensions.Configuration;

namespace Securrency.TDS.Web.DataLayer
{
    public class DataLayerOptions
    {
        public string ConnectionString { get; set; }

        public static IConfiguration From(IConfiguration config)
        {
            return config.GetSection("Database");
        }
    }
}